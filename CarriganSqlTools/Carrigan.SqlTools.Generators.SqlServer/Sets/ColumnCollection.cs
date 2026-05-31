using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.IdentifierTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Carrigan.SqlTools.Sets;

public class ColumnCollection<T> : ColumnCollectionBase<T> where T : class
{
    public ColumnCollection(params IEnumerable<PropertyName> propertyNames) : base(propertyNames)
    {
    }
    public ColumnCollection(params IEnumerable<string> propertyNames) : base(propertyNames.Select(propertyName => new PropertyName(propertyName)))
    {
    }

    protected override HashSet<Type> SupportedTypes =>
        DialectStatics.SupportedTypes;

    protected override ColumnCollection<T> FromPropertyName(IEnumerable<PropertyName> propertyNames) =>
        new (propertyNames);

    public override ColumnCollection<T> AppendColumn(PropertyName propertyName) =>
        (ColumnCollection<T>)base.AppendColumn(propertyName);

    public override ColumnCollection<T> AppendColumn(string propertyName) =>
        AppendColumn(new PropertyName(propertyName));

    public override ColumnCollection<T> ConcatColumn(params IEnumerable<PropertyName> propertyNames) =>
        (ColumnCollection<T>)base.ConcatColumn(propertyNames);

    public override ColumnCollection<T> ConcatColumn(params IEnumerable<string> propertyNames) =>
        ConcatColumn(propertyNames.Select(static propertyName => new PropertyName(propertyName)));

}
