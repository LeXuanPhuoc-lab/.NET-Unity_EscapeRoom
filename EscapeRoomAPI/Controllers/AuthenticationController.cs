using AutoMapper;
using EscapeRoomAPI.Entities;
using EscapeRoomAPI.Extensions;
using EscapeRoomAPI.Payloads;
using EscapeRoomAPI.Payloads.Requests;
using EscapeRoomAPI.Payloads.Responses;
using EscapeRoomAPI.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EscapeRoomAPI.Controllers;

[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly EscapeRoomUnityContext _context;
    private readonly IMapper _mapper;

    public AuthenticationController(EscapeRoomUnityContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }


    [HttpPost(APIRoutes.Authentication.SignIn, Name = nameof(SignInAsync))]
    public async Task<IActionResult> SignInAsync([FromBody] SignInRequest reqObj)
    {
        // Process sign in validation 
        var validationResult = await reqObj.ValidateAsync();
        if (validationResult is not null) // Invoke errors
        {
            return BadRequest(new BaseResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                IsSuccess = false,
                Errors = validationResult.Errors
            });
        }

        // Process sign in 
        // Get player by username & password
        var existingPlayer = await _context.Players.FirstOrDefaultAsync(
                x => x.Username.Equals(reqObj.Username) && x.Password.Equals(reqObj.Password)
        );

        return existingPlayer is not null // sign in success
            ? Ok(new BaseResponse { StatusCode = StatusCodes.Status200OK, Message = "Đăng nhập thành công, chiến thôi!!!" })
            : Unauthorized();
    }

    [HttpPost(APIRoutes.Authentication.Register, Name = nameof(RegisterAsync))]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest reqObj)
    {
        // Process register validation 
        var validationResult = await reqObj.ValidateAsync();
        if (validationResult is not null) // Invoke errors
        {
            return BadRequest(new BaseResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                IsSuccess = false,
                Errors = validationResult.Errors
            });
        }

        // Convert to typeof(Entity)
        var playerEntity = _mapper.Map<Player>(reqObj.ToPlayerDto());
        // Add new player 
        _context.Players.Add(playerEntity);
        var result = await _context.SaveChangesAsync() > 0; 

        return result 
            ? Ok(new BaseResponse{ StatusCode = StatusCodes.Status200OK, Message = "Tạo tài khoản thành công, chơi vui nhé!"})
            : Problem("Có lỗi xảy ra", null, StatusCodes.Status500InternalServerError);
    }
}