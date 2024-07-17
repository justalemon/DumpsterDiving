using System;
using Newtonsoft.Json;

namespace DumpsterDiving;

/// <summary>
/// An invalid converter that does nothing.
/// </summary>
public class InvalidConverter : JsonConverter
{
    #region Properties

    /// <inheritdoc/>
    public override bool CanRead => false;
    /// <inheritdoc/>
    public override bool CanWrite => false;

    #endregion

    #region Functions

    /// <inheritdoc/>
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();
    /// <inheritdoc/>
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) => throw new NotImplementedException();
    /// <inheritdoc/>
    public override bool CanConvert(Type objectType) => throw new NotImplementedException();

    #endregion
}