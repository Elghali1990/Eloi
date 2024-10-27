using E.Loi.NodeJsonVm;

namespace E.Loi.Services;

public class ReadJsonFileService(IWebHostEnvironment _webHostEnvironment)
{
    public List<PhaseVm> GetPhasesFromJsonFile()
    {
        List<PhaseVm> phases = new();
        string growersJson = File.ReadAllText(_webHostEnvironment.WebRootPath + "/Data/Phase.json");
        phases = System.Text.Json.JsonSerializer.Deserialize<List<PhaseVm>>(growersJson)!.ToList();
        return phases;
    }

    public List<PhaseVm> GetPhasesConsultationFromJsonFile()
    {
        List<PhaseVm> phases = new();
        string growersJson = File.ReadAllText(_webHostEnvironment.WebRootPath + "/Data/PhaseConsultation.json");
        phases = System.Text.Json.JsonSerializer.Deserialize<List<PhaseVm>>(growersJson)!.ToList();
        return phases;
    }

    public Root GetNodeDataFromContent(string stringJson)
    {
        Root root = new();
        root = System.Text.Json.JsonSerializer.Deserialize<Root>(stringJson)!;
        return root;
    }
}
