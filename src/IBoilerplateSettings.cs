namespace LostTech.App {
    public interface IBoilerplateSettings {
        string? AcceptedTerms { get; set; }
        string? WhatsNewVersionSeen { get; set; }
        bool? ReportCrashes { get; set; }
        bool? EnableTelemetry { get; set; }
    }
}
