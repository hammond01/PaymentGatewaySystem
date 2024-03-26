using Newtonsoft.Json.Serialization;

namespace PaymentGateway.Ultils.Extension;

public class RemovePrefixContractResolver : DefaultContractResolver
{
    protected override string ResolvePropertyName(string propertyName)
    {
        // Loại bỏ tất cả các ký tự "vnp_" từ tên thuộc tính
        if (propertyName.StartsWith("vnp_")) return propertyName.Substring(4);
        return base.ResolvePropertyName(propertyName);
    }
}