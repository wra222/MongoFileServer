using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MonGo.Entity
{
    [DataContract]
    public class ApiResponse
    {
        [DataMember(Name = "state", EmitDefaultValue = true, IsRequired = true, Order = 1)]
        public string State { get; set; }
        [DataMember(Name = "content", IsRequired = true, Order = 2)]
        public List<Content> Content;
    }
}
