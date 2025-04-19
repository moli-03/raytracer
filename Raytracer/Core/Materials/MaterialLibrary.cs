namespace Raytracer.Core.Materials;

public class MaterialLibrary
{
    // Private static fields to hold the single instance of each material
    private static Emerald? _emerald;
    private static Jade? _jade;
    private static Obsidian? _obsidian;
    private static Pearl? _pearl;
    private static Ruby? _ruby;
    private static Turquoise? _turquoise;
    private static Brass? _brass;
    private static Bronze? _bronze;
    private static Chrome? _chrome;
    private static Copper? _copper;
    private static Gold? _gold;
    private static Silver? _silver;
    private static BlackPlastic? _blackPlastic;
    private static CyanPlastic? _cyanPlastic;
    private static GreenPlastic? _greenPlastic;
    private static RedPlastic? _redPlastic;
    private static WhitePlastic? _whitePlastic;
    private static YellowPlastic? _yellowPlastic;
    private static Glass? _glass;

    // Public static properties that return the same instance each time
    public static Emerald Emerald => _emerald ??= new Emerald();
    public static Jade Jade => _jade ??= new Jade();
    public static Obsidian Obsidian => _obsidian ??= new Obsidian();
    public static Pearl Pearl => _pearl ??= new Pearl();
    public static Ruby Ruby => _ruby ??= new Ruby();
    public static Turquoise Turquoise => _turquoise ??= new Turquoise();
    public static Brass Brass => _brass ??= new Brass();
    public static Bronze Bronze => _bronze ??= new Bronze();
    public static Chrome Chrome => _chrome ??= new Chrome();
    public static Copper Copper => _copper ??= new Copper();
    public static Gold Gold => _gold ??= new Gold();
    public static Silver Silver => _silver ??= new Silver();
    public static BlackPlastic BlackPlastic => _blackPlastic ??= new BlackPlastic();
    public static CyanPlastic CyanPlastic => _cyanPlastic ??= new CyanPlastic();
    public static GreenPlastic GreenPlastic => _greenPlastic ??= new GreenPlastic();
    public static RedPlastic RedPlastic => _redPlastic ??= new RedPlastic();
    public static WhitePlastic WhitePlastic => _whitePlastic ??= new WhitePlastic();
    public static YellowPlastic YellowPlastic => _yellowPlastic ??= new YellowPlastic();
    public static Glass Glass => _glass ??= new Glass();
}
