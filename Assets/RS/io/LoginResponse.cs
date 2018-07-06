namespace RS
{
    /// <summary>
    /// Represents many login responses.
    /// </summary>
    public enum LoginResponse
    {
        /// <summary>
        /// The login was successful.
        /// </summary>
        SuccessfulLogin,
        /// <summary>
        /// An invalid username or password was provided.
        /// </summary>
        InvalidCredentials,
        /// <summary>
        /// Your account was disabled.
        /// </summary>
        AccountDisabled,
        /// <summary>
        /// The response was unknown.
        /// </summary>
        Unknown,
    }
}
