using System.Globalization;
using Simansoft.BridgeBank.Parsers.Util;

namespace BridgeBank.Parsers.Tests;

/// <summary>
/// Testes unitários para o parseador de valores monetários
/// </summary>
public class ParseadorNumericoTests
{
    [Theory]
    [InlineData("4.400,00", "4400.00")]
    [InlineData("-318.112,16", "-318112.16")]
    [InlineData("+200,00", "200.00")]
    [InlineData("1.234.567,89", "1234567.89")]
    [InlineData("7,84", "7.84")]
    [InlineData("-0,16", "-0.16")]
    public void ParsearValorMonetario_FormatoPortugues_RetornaValorCorreto(string texto, string esperado)
    {
        decimal resultado = ParseadorNumerico.ParsearValorMonetario(texto);
        Assert.Equal(decimal.Parse(esperado, CultureInfo.InvariantCulture), resultado);
    }

    [Theory]
    [InlineData("+200,00 MZN", "200.00")]
    [InlineData("+31.552,00 MZN", "31552.00")]
    [InlineData("8.965.395,17 ", "8965395.17")]
    public void ParsearValorMonetario_ComSufixoMoeda_RetornaValorCorreto(string texto, string esperado)
    {
        decimal resultado = ParseadorNumerico.ParsearValorMonetario(texto);
        Assert.Equal(decimal.Parse(esperado, CultureInfo.InvariantCulture), resultado);
    }

    [Theory]
    [InlineData("3.371.027.03", "3371027.03")]
    [InlineData("132829.99", "132829.99")]
    [InlineData("1234.56", "1234.56")]
    public void ParsearValorMonetario_FormatoComPontos_RetornaValorCorreto(string texto, string esperado)
    {
        decimal resultado = ParseadorNumerico.ParsearValorMonetario(texto);
        Assert.Equal(decimal.Parse(esperado, CultureInfo.InvariantCulture), resultado);
    }

    [Theory]
    [InlineData("3 371 027.03", "3371027.03")]
    [InlineData("132 829.99", "132829.99")]
    [InlineData("4 400.00", "4400.00")]
    public void ParsearValorMonetario_ComEspacosComoMilhares_RetornaValorCorreto(string texto, string esperado)
    {
        decimal resultado = ParseadorNumerico.ParsearValorMonetario(texto);
        Assert.Equal(decimal.Parse(esperado, CultureInfo.InvariantCulture), resultado);
    }

    [Theory]
    [InlineData(null, "0")]
    [InlineData("", "0")]
    [InlineData("  ", "0")]
    public void ParsearValorMonetario_ValorVazioOuNulo_RetornaZero(string? texto, string esperado)
    {
        decimal resultado = ParseadorNumerico.ParsearValorMonetario(texto);
        Assert.Equal(decimal.Parse(esperado, CultureInfo.InvariantCulture), resultado);
    }

    [Theory]
    [InlineData("1,234,567.89", "1234567.89")]
    [InlineData("1,234.56", "1234.56")]
    public void ParsearValorMonetario_FormatoInternacional_RetornaValorCorreto(string texto, string esperado)
    {
        decimal resultado = ParseadorNumerico.ParsearValorMonetario(texto);
        Assert.Equal(decimal.Parse(esperado, CultureInfo.InvariantCulture), resultado);
    }
}