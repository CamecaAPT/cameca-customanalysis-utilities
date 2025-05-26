using System.ComponentModel.DataAnnotations;

namespace Cameca.CustomAnalysis.Utilities;

public enum Element
{
    [Display(Name = "Hydrogen")]
    H = 1,
    [Display(Name = "Helium")]
    He,
    [Display(Name = "Lithium")]
    Li,
    [Display(Name = "Beryllium")]
    Be,
    [Display(Name = "Boron")]
    B,
    [Display(Name = "Carbon")]
    C,
    [Display(Name = "Nitrogen")]
    N,
    [Display(Name = "Oxygen")]
    O,
    [Display(Name = "Fluorine")]
    F,
    [Display(Name = "Neon")]
    Ne,
    [Display(Name = "Sodium")]
    Na,
    [Display(Name = "Magnesium")]
    Mg,
    [Display(Name = "Aluminium")]
    Al,
    [Display(Name = "Silicon")]
    Si,
    [Display(Name = "Phosphorus")]
    P,
    [Display(Name = "Sulfur")]
    S,
    [Display(Name = "Chlorine")]
    Cl,
    [Display(Name = "Argon")]
    Ar,
    [Display(Name = "Potassium")]
    K,
    [Display(Name = "Calcium")]
    Ca,
    [Display(Name = "Scandium")]
    Sc,
    [Display(Name = "Titanium")]
    Ti,
    [Display(Name = "Vanadium")]
    V,
    [Display(Name = "Chromium")]
    Cr,
    [Display(Name = "Manganese")]
    Mn,
    [Display(Name = "Iron")]
    Fe,
    [Display(Name = "Cobalt")]
    Co,
    [Display(Name = "Nickel")]
    Ni,
    [Display(Name = "Copper")]
    Cu,
    [Display(Name = "Zinc")]
    Zn,
    [Display(Name = "Gallium")]
    Ga,
    [Display(Name = "Germanium")]
    Ge,
    [Display(Name = "Arsenic")]
    As,
    [Display(Name = "Selenium")]
    Se,
    [Display(Name = "Bromine")]
    Br,
    [Display(Name = "Krypton")]
    Kr,
    [Display(Name = "Rubidium")]
    Rb,
    [Display(Name = "Strontium")]
    Sr,
    [Display(Name = "Yttrium")]
    Y,
    [Display(Name = "Zirconium")]
    Zr,
    [Display(Name = "Niobium")]
    Nb,
    [Display(Name = "Molybdenum")]
    Mo,
    [Display(Name = "Technetium")]
    Tc,
    [Display(Name = "Ruthenium")]
    Ru,
    [Display(Name = "Rhodium")]
    Rh,
    [Display(Name = "Palladium")]
    Pd,
    [Display(Name = "Silver")]
    Ag,
    [Display(Name = "Cadmium")]
    Cd,
    [Display(Name = "Indium")]
    In,
    [Display(Name = "Tin")]
    Sn,
    [Display(Name = "Antimony")]
    Sb,
    [Display(Name = "Tellurium")]
    Te,
    [Display(Name = "Iodine")]
    I,
    [Display(Name = "Xenon")]
    Xe,
    [Display(Name = "Caesium")]
    Cs,
    [Display(Name = "Barium")]
    Ba,
    [Display(Name = "Lanthanum")]
    La,
    [Display(Name = "Cerium")]
    Ce,
    [Display(Name = "Praseodymium")]
    Pr,
    [Display(Name = "Neodymium")]
    Nd,
    [Display(Name = "Promethium")]
    Pm,
    [Display(Name = "Samarium")]
    Sm,
    [Display(Name = "Europium")]
    Eu,
    [Display(Name = "Gadolinium")]
    Gd,
    [Display(Name = "Terbium")]
    Tb,
    [Display(Name = "Dysprosium")]
    Dy,
    [Display(Name = "Holmium")]
    Ho,
    [Display(Name = "Erbium")]
    Er,
    [Display(Name = "Thulium")]
    Tm,
    [Display(Name = "Ytterbium")]
    Yb,
    [Display(Name = "Lutetium")]
    Lu,
    [Display(Name = "Hafnium")]
    Hf,
    [Display(Name = "Tantalum")]
    Ta,
    [Display(Name = "Tungsten")]
    W,
    [Display(Name = "Rhenium")]
    Re,
    [Display(Name = "Osmium")]
    Os,
    [Display(Name = "Iridium")]
    Ir,
    [Display(Name = "Platinum")]
    Pt,
    [Display(Name = "Gold")]
    Au,
    [Display(Name = "Mercury")]
    Hg,
    [Display(Name = "Thallium")]
    Tl,
    [Display(Name = "Lead")]
    Pb,
    [Display(Name = "Bismuth")]
    Bi,
    [Display(Name = "Polonium")]
    Po,
    [Display(Name = "Astatine")]
    At,
    [Display(Name = "Radon")]
    Rn,
    [Display(Name = "Francium")]
    Fr,
    [Display(Name = "Radium")]
    Ra,
    [Display(Name = "Actinium")]
    Ac,
    [Display(Name = "Thorium")]
    Th,
    [Display(Name = "Protactinium")]
    Pa,
    [Display(Name = "Uranium")]
    U,
    [Display(Name = "Neptunium")]
    Np,
    [Display(Name = "Plutonium")]
    Pu,
    [Display(Name = "Americium")]
    Am,
    [Display(Name = "Curium")]
    Cm,
    [Display(Name = "Berkelium")]
    Bk,
    [Display(Name = "Californium")]
    Cf,
    [Display(Name = "Einsteinium")]
    Es,
    [Display(Name = "Fermium")]
    Fm,
    [Display(Name = "Mendelevium")]
    Md,
    [Display(Name = "Nobelium")]
    No,
    [Display(Name = "Lawrencium")]
    Lr,
    [Display(Name = "Rutherfordium")]
    Rf,
    [Display(Name = "Dubnium")]
    Db,
    [Display(Name = "Seaborgium")]
    Sg,
    [Display(Name = "Bohrium")]
    Bh,
    [Display(Name = "Hassium")]
    Hs,
    [Display(Name = "Meitnerium")]
    Mt,
    [Display(Name = "Darmstadtium")]
    Ds,
    [Display(Name = "Roentgenium")]
    Rg,
    [Display(Name = "Copernicium")]
    Cn,
    [Display(Name = "Nihonium")]
    Nh,
    [Display(Name = "Flerovium")]
    Fl,
    [Display(Name = "Moscovium")]
    Mc,
    [Display(Name = "Livermorium")]
    Lv,
    [Display(Name = "Tennessine")]
    Ts,
    [Display(Name = "Oganesson")]
    Og,
}
