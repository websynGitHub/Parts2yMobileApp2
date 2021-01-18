using System.Collections.Generic;

namespace YPS.Model
{
    public class SafetyNetModel
    {
        public string nonce { get; set; }
        public long timestampMs { get; set; }
        public string apkPackageName { get; set; }
        public string apkDigestSha256 { get; set; }
        public string ctsProfileMatch { get; set; }
        public List<string> apkCertificateDigestSha256 { get; set; }
        public string basicIntegrity { get; set; }
        public string advice { get; set; }
        public string evaluationType { get; set; }
    }
}
