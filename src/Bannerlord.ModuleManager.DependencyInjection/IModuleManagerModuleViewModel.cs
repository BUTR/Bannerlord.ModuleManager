namespace Bannerlord.ModuleManager.DependencyInjection;

public interface IModuleManagerModuleViewModel
{
    bool GetSelected(string moduleId);
    void SetSelected(string moduleId, bool value);
    
    bool GetDisabled(string moduleId);
    void SetDisabled(string moduleId, bool value);
}