namespace Devocean.Aws.Sns;

public enum SmsType
{
    /// <summary>
    /// For Noncritical Messages => lowest cost
    /// </summary>
    Promotional = 1, // => lowest cost
    /// <summary>
    /// For Critical Messages => highest reliability
    /// </summary>
    Transactional = 2 // => Reliability
}