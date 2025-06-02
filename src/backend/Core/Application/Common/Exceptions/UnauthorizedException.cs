using System.Net;

namespace EvrenDev.Application.Common.Exceptions;

public class UnauthorizedException(string message) : CustomException(message, null, HttpStatusCode.Unauthorized);
