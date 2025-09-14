namespace Orama.Modules;

/// <summary>
/// The base for all Orama modules.
/// </summary>
public class Module
{
	public bool Enabled { get; private set; } = true;
	
	public virtual void Start() { }
	public virtual void Update() { }
	
	public void Activate() => Enabled = true;
	public void Deactivate() => Enabled = false;
}