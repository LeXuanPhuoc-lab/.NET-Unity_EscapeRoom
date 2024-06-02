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
    private readonly IServiceProvider _serviceProvider;

    public AuthenticationController(EscapeRoomUnityContext context,
        IMapper mapper,
        IServiceProvider serviceProvider)
    {
        _context = context;
        _mapper = mapper;
        _serviceProvider = serviceProvider;
    }


    [HttpPost(APIRoutes.Authentication.SignIn, Name = nameof(SignInAsync))]
    public async Task<IActionResult> SignInAsync([FromBody] SignInRequest reqObj)
    {
        // Process sign in validation 
        var validationResult = await reqObj.ValidateAsync(_serviceProvider);
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
            ? Ok(new BaseResponse
                { StatusCode = StatusCodes.Status200OK, Message = "Đăng nhập thành công, chiến thôi!!!" })
            : Unauthorized(new BaseResponse
                { StatusCode = StatusCodes.Status401Unauthorized, Message = "Sai username hoặc password" });
    }

    [HttpPost(APIRoutes.Authentication.Register, Name = nameof(RegisterAsync))]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest reqObj)
    {
        // Process register validation 
        var validationResult = await reqObj.ValidateAsync(_serviceProvider);
        if (validationResult is not null) // Invoke errors
        {
            return BadRequest(new BaseResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                IsSuccess = false,
                Errors = validationResult.Errors
            });
        }

        // Check exist username
        var player = await _context.Players.FirstOrDefaultAsync(x => x.Username.Equals(reqObj.Username));
        if (player is not null)
        {
            return BadRequest(new BaseResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Tên tài khoản đã tồn tại"
            });
        }

        // Convert to typeof(Entity)
        var playerEntity = _mapper.Map<Player>(reqObj.ToPlayerDto());
        // Add new player 
        await _context.Players.AddAsync(playerEntity);
        var result = await _context.SaveChangesAsync() > 0;

        return result
            ? Ok(new BaseResponse
                { StatusCode = StatusCodes.Status200OK, Message = "Tạo tài khoản thành công, chơi vui nhé!" })
            : Problem("Có lỗi xảy ra", null, StatusCodes.Status500InternalServerError);
    }
}