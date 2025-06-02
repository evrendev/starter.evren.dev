using System.Net;

namespace EvrenDev.Application.Common.Exceptions;

public class ForbiddenException(string message) : CustomException(message, null, HttpStatusCode.Forbidden);
