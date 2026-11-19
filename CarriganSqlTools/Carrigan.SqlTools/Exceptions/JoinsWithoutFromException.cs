
namespace Carrigan.SqlTools.Exceptions;

public sealed class JoinsWithoutFromException()
    : InvalidOperationException("A SELECT statement cannot contain joins without a FROM clause.");
