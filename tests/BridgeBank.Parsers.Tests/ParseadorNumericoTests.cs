using System.Globalization;
using Simansoft.BridgeBank.Parsers.Util;

namespace BridgeBank.Parsers.Tests;

/// <summary>
/// Testes unitários para o parseador de valores monetários
/// </summary>
[TestClass]
public class ParseadorNumericoTests
{
    [TestMethod]
    [DataRow("4.400,00", "4400.00")]
    [DataRow("-318.112,16", "-318112.16")]
    [DataRow("+200,00", "200.00")]
    [DataRow("1.234.567,89", "1234567.89")]
    [DataRow("7,84", "7.84")]
    [DataRow("-0,16", "-0.16")]
    public void ParsearValorMonetario_FormatoPortugues_RetornaValorCorreto(string texto, string esperado)
    {
        decimal resultado = ParseadorNumerico.ParsearValorMonetario(texto);
        Assert.AreEqual(decimal.Parse(esperado, CultureInfo.InvariantCulture), resultado);
    }

    [TestMethod]
    [DataRow("+200,00 MZN", "200.00")]
    [DataRow("+31.552,00 MZN", "31552.00")]
    [DataRow("8.965.395,17 ", "8965395.17")]
    public void ParsearValorMonetario_ComSufixoMoeda_RetornaValorCorreto(string texto, string esperado)
    {
        decimal resultado = ParseadorNumerico.ParsearValorMonetario(texto);
        Assert.AreEqual(decimal.Parse(esperado, CultureInfo.InvariantCulture), resultado);
    }

    [TestMethod]
    [DataRow("3.371.027.03", "3371027.03")]
    [DataRow("132829.99", "132829.99")]
    [DataRow("1234.56", "1234.56")]
    public void ParsearValorMonetario_FormatoComPontos_RetornaValorCorreto(string texto, string esperado)
    {
        decimal resultado = ParseadorNumerico.ParsearValorMonetario(texto);
        Assert.AreEqual(decimal.Parse(esperado, CultureInfo.InvariantCulture), resultado);
    }

    [TestMethod]
    [DataRow("3 371 027.03", "3371027.03")]
    [DataRow("132 829.99", "132829.99")]
    [DataRow("4 400.00", "4400.00")]
    public void ParsearValorMonetario_ComEspacosComoMilhares_RetornaValorCorreto(string texto, string esperado)
    {
        decimal resultado = ParseadorNumerico.ParsearValorMonetario(texto);
        Assert.AreEqual(decimal.Parse(esperado, CultureInfo.InvariantCulture), resultado);
    }

    [TestMethod]
    [DataRow(null, "0")]
    [DataRow("", "0")]
    [DataRow("  ", "0")]
    public void ParsearValorMonetario_ValorVazioOuNulo_RetornaZero(string? texto, string esperado)
    {
        decimal resultado = ParseadorNumerico.ParsearValorMonetario(texto);
        Assert.AreEqual(decimal.Parse(esperado, CultureInfo.InvariantCulture), resultado);
    }

    [TestMethod]
    [DataRow("1,234,567.89", "1234567.89")]
    [DataRow("1,234.56", "1234.56")]
    public void ParsearValorMonetario_FormatoInternacional_RetornaValorCorreto(string texto, string esperado)
    {
        decimal resultado = ParseadorNumerico.ParsearValorMonetario(texto);
        Assert.AreEqual(decimal.Parse(esperado, CultureInfo.InvariantCulture), resultado);
    }
}