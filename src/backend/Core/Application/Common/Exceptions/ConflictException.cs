using System.Net;

namespace EvrenDev.Application.Common.Exceptions;

public class ConflictException(string message) : CustomException(message, null, HttpStatusCode.Conflict);
