
namespace nhautomapprog.Models
{
    /// <summary>
    /// NHibernate stores enums with their numeric value into the database as default. 
    /// For NHibernate enums are primitive values, and not "entity objects".
    /// To map this enum's values as string, the related property should have the following type:
    /// type="NHibernate.Type.EnumStringType`1[[NH32AutoMap.Models.Level, NH32AutoMap]]" 
    /// and EnumConvention class will take care of that!
    /// 
    /// It's recommended to map enums to strings instead of dafault integers,
    /// because your Enum declaration is not dynamic, or simpler, it doesn't change without recompiling.
    /// Another reason is if you change the Enum (in code), you'd have to synchronize it with the database table. 
    /// Since Enums don't have an incremental key (PK), they can't be synchronized so simple. 
    /// For example if you remove the "Bronze" item, then the "Silver" item would have the value of zero.
    /// </summary>
    public enum Level
    {
        Bronze,
        Silver,
        Gold
    }
}
