using System.Net;

namespace EvrenDev.Application.Common.Exceptions;

public class NotFoundException(string message) : CustomException(message, null, HttpStatusCode.NotFound);
