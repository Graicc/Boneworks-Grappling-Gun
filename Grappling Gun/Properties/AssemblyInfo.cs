using System.Resources;
using System.Reflection;
using System.Runtime.InteropServices;
using MelonLoader;

[assembly: AssemblyTitle(Grappling_Gun.BuildInfo.Name)]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(Grappling_Gun.BuildInfo.Company)]
[assembly: AssemblyProduct(Grappling_Gun.BuildInfo.Name)]
[assembly: AssemblyCopyright("Created by " + Grappling_Gun.BuildInfo.Author)]
[assembly: AssemblyTrademark(Grappling_Gun.BuildInfo.Company)]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
//[assembly: Guid("")]
[assembly: AssemblyVersion(Grappling_Gun.BuildInfo.Version)]
[assembly: AssemblyFileVersion(Grappling_Gun.BuildInfo.Version)]
[assembly: NeutralResourcesLanguage("en")]
[assembly: MelonModInfo(typeof(Grappling_Gun.Grappling_Gun), Grappling_Gun.BuildInfo.Name, Grappling_Gun.BuildInfo.Version, Grappling_Gun.BuildInfo.Author, Grappling_Gun.BuildInfo.DownloadLink)]


// Create and Setup a MelonModGame to mark a Mod as Universal or Compatible with specific Games.
// If no MelonModGameAttribute is found or any of the Values for any MelonModGame on the Mod is null or empty it will be assumed the Mod is Universal.
// Values for MelonModGame can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
[assembly: MelonModGame(null, null)]