using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class SearchPathAttribute : Attribute
{
	public string path;

	public SearchPathAttribute(string path)
	{
		this.path = path;
	}
}