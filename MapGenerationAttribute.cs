using System;

public class MapGenerationAttribute : Attribute
{
	public bool Delete { get; set; }

	public Type[] KeepComponents { get; set; } = Array.Empty<Type>();

}
