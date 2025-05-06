
using System;
using System.Text.Json.Serialization;


namespace MelodiaTherapy.Models;
public class SubscriptionModel
{
    [JsonPropertyName("isPromotion")]
    public bool IsPromotion { get; set; }

    [JsonPropertyName("subscriptionCode")]
    public string SubscriptionCode { get; set; }

    [JsonPropertyName("parentSubscriptionCode")]
    public string? ParentSubscriptionCode { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    public override string ToString()
    {
        return $"Subscription{{isPromotion: {IsPromotion}, subscriptionCode: {SubscriptionCode}, parentSubscriptionCode: {ParentSubscriptionCode}, description: {Description}}}";
    }
}
