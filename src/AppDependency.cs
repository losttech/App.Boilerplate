namespace LostTech.App;

using System;
using System.Collections.Generic;

public class AppDependecy {
    public AppDependecy(string name, Uri? uri = null, string? license = null) {
        this.Name = name ?? throw new ArgumentNullException(nameof(name));
        this.Authors = $"contributors to {this.Name}";
        this.Uri = uri;
        this.License = license;
    }

    public AppDependecy(string name, string uri, string? license = null) : this(name, license: license) {
        if (uri != null)
            this.Uri = new Uri(uri);
    }

    public string Name { get; set; }
    public string Authors { get; set; }
    public string? License { get; set; }
    public Uri? Uri { get; set; }

    public static IEnumerable<AppDependecy> Dependencies { get; } = Array.Empty<AppDependecy>();
}
