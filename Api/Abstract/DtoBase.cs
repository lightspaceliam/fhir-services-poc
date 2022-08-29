using System.Runtime.Serialization;

namespace Api
{
    [DataContract]
    public class DtoBase : IDto
    {
        [DataMember]
        public Guid Id { get; set; }
    }
}
