// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;

namespace System.Windows.Forms.Design;

/// <summary>
/// The predefined mask descriptors to be shown in the mask designer dialog.
/// </summary>
internal class MaskDescriptorTemplate : MaskDescriptor
{
    public MaskDescriptorTemplate(string? mask, string name, string? sample, Type? validatingType, CultureInfo culture)
        : this(mask, name, sample, validatingType, culture, skipValidation: false)
    {
    }

    public MaskDescriptorTemplate(string? mask, string name, string? sample, Type? validatingType, CultureInfo? culture, bool skipValidation)
    {
        Mask = mask;
        Name = name;
        Sample = sample;
        ValidatingType = validatingType;
        Culture = culture!;

        if (skipValidation)
        {
            return;
        }

        if (!IsValidMaskDescriptor(this))
        {
            // Don't throw here, callers should check the Mask property for validity. See the ValidMaskDescriptorList below.
            Mask = null;
        }
    }

    public override string? Mask { get; }

    public override string Name { get; }

    public override string? Sample { get; }

    public override Type? ValidatingType { get; }

    public override CultureInfo Culture { get; }

    /// <summary>
    /// Get the canned mask descriptors according to the specified culture.
    /// </summary>
    public static List<MaskDescriptor> GetLocalizedMaskDescriptors(CultureInfo culture)
    {
        ValidMaskDescriptorList maskDescriptors = new();

        // Note: MaskDescriptor.Sample should not contain culture-sensitive literals since they can fail the mask
        //       for some specific cultures. (ex: date separator).

        switch (culture.Parent.Name)
        {
            default:
                culture = CultureInfo.InvariantCulture;
                goto case "en";

            // case "en-US": // English US.
            case "en": // English.
                // Numeric.
                maskDescriptors.Add(new MaskDescriptorTemplate("00000", "Numeric (5-digits)", "12345", typeof(int), culture));
                // Phone Number.
                maskDescriptors.Add(new MaskDescriptorTemplate("(999) 000-0000", "Phone number", "5745550123", null, culture));
                // Phone Number No Area Code.
                maskDescriptors.Add(new MaskDescriptorTemplate("000-0000", "Phone number no area code", "5550123", null, culture));
                // Short Date.
                maskDescriptors.Add(new MaskDescriptorTemplate("00/00/0000", "Short date", "12112003", typeof(DateTime), culture));
                // Short Date and Time.
                maskDescriptors.Add(new MaskDescriptorTemplate("00/00/0000 90:00", "Short date and time (US)", "121120031120", typeof(DateTime), culture));
                // Social Security Number.
                maskDescriptors.Add(new MaskDescriptorTemplate("000-00-0000", "Social security number", "000001234", null, culture));
                // Time.
                maskDescriptors.Add(new MaskDescriptorTemplate("90:00", "Time (US)", "1120", typeof(DateTime), culture));
                // Time 24.
                maskDescriptors.Add(new MaskDescriptorTemplate("00:00", "Time (European/Military)", "2320", typeof(DateTime), culture));
                // Zip code.
                maskDescriptors.Add(new MaskDescriptorTemplate("00000-9999", "Zip Code", "980526399", null, culture));
                break;

            // case "ar-SA": // Arabic Saudi Arabia.
            case "ar": // Arabic.
                // Phone Number.
                maskDescriptors.Add(new MaskDescriptorTemplate("(999)000-0000", "Phone Number", "0123456789", null, culture));
                // Phone Number No Area Code.
                maskDescriptors.Add(new MaskDescriptorTemplate("000-0000", "Phone Number no Area Code", "1234567", null, culture));
                // Short Date.
                maskDescriptors.Add(new MaskDescriptorTemplate("00 /00 /0000", "Short Date", "26102005", typeof(DateTime), culture));
                // Short Date and Time.
                maskDescriptors.Add(new MaskDescriptorTemplate("00 /00 /0000 00:00", "Short Date/Time", "261020051430", typeof(DateTime), culture));
                // Social Security Number.
                maskDescriptors.Add(new MaskDescriptorTemplate("000-00-0000", "Social Security Number", "123456789", null, culture));
                // Time.
                maskDescriptors.Add(new MaskDescriptorTemplate("90:00", "Time", " 230", typeof(DateTime), culture));
                // Time 24.
                maskDescriptors.Add(new MaskDescriptorTemplate("00:00", "Time (24 Hour)", "1430", typeof(DateTime), culture));
                break;

            // case "de-DE": // German Germany.
            case "de": // German.
                // Short Date.
                maskDescriptors.Add(new MaskDescriptorTemplate("00/00/0000", "Datum kurz", "28112005", typeof(DateTime), culture));
                // Short Date and Time.
                maskDescriptors.Add(new MaskDescriptorTemplate("00/00/0000 00:00", "Datum lang", "281120051430", typeof(DateTime), culture));
                // Time.
                maskDescriptors.Add(new MaskDescriptorTemplate("90:00", "Zeit", "1430", typeof(DateTime), culture));
                // Zip code.
                maskDescriptors.Add(new MaskDescriptorTemplate("00000", "Postleitzahl", "91450", null, culture));
                break;

            // case "fr-FR": // French.
            case "fr": // French.
                // Special-case date sample format for French-Canada.
                string dateSample = culture.Name == "fr-CA" ? "11282005" : "28112005";
                // Numeric.
                maskDescriptors.Add(new MaskDescriptorTemplate("99999", "Numérique (5 chiffres)", "12345", typeof(int), culture));
                // Phone Number.
                maskDescriptors.Add(new MaskDescriptorTemplate("00 00 00 00 00 00", "Numéro de téléphone (France)", "0123456789", null, culture));
                // Short Date.
                maskDescriptors.Add(new MaskDescriptorTemplate("00/00/0000", "Date (format court)", dateSample, typeof(DateTime), culture));
                // Short Date and Time.
                maskDescriptors.Add(new MaskDescriptorTemplate("00/00/0000 00:00", "Date et heure (format long)", dateSample + "1430", typeof(DateTime), culture));
                // Social Security Number.
                maskDescriptors.Add(new MaskDescriptorTemplate("0 00 00 00 000 000 00", "Numéro de Sécurité Sociale (France)", "163117801234548", null, culture));
                // Time.
                maskDescriptors.Add(new MaskDescriptorTemplate("00:00", "Heure", "1430", typeof(DateTime), culture));
                // Zip code.
                maskDescriptors.Add(new MaskDescriptorTemplate("00000", "Code postal (France)", "91450", null, culture));
                break;

            // case "it-IT": // Italian Italy.
            case "it": // Italian.
                // Numeric.
                maskDescriptors.Add(new MaskDescriptorTemplate("99999", "Numerico (5 Cifre)", "12345", typeof(int), culture));
                // Phone Number.
                maskDescriptors.Add(new MaskDescriptorTemplate("0000 00000", "Numero di telefono", "012345678", null, culture));
                // Mobile Phone Number.
                maskDescriptors.Add(new MaskDescriptorTemplate("000 0000000", "Numero di cellulare", "1234567890", null, culture));
                // Short Date.
                maskDescriptors.Add(new MaskDescriptorTemplate("00/00/0000", "Data breve", "26102005", typeof(DateTime), culture));
                // Short Date and Time.
                maskDescriptors.Add(new MaskDescriptorTemplate("00/00/0000 00:00", "Data e ora", "261020051430", typeof(DateTime), culture));
                // Time.
                maskDescriptors.Add(new MaskDescriptorTemplate("00:00", "Ora", "1430", typeof(DateTime), culture));
                // Zip code.
                maskDescriptors.Add(new MaskDescriptorTemplate("00000", "Codice postale", "12345", null, culture));
                break;

            // case "es-ES": // Spanish Spain.
            case "es": // Spanish.
                // Numeric.
                maskDescriptors.Add(new MaskDescriptorTemplate("99999", "Numérico", "12345", typeof(int), culture));
                // Phone Number.
                maskDescriptors.Add(new MaskDescriptorTemplate("(999)000-0000", "Número de teléfono", "0123456789", null, culture));
                // Mobile Phone Number.
                maskDescriptors.Add(new MaskDescriptorTemplate("000-000-0000", "Número de teléfono móvil", "0001234567", null, culture));
                // Phone Number No Area Code.
                maskDescriptors.Add(new MaskDescriptorTemplate("000-0000", "Número de teléfono sin código de área", "1234567", null, culture));
                // Short Date.
                maskDescriptors.Add(new MaskDescriptorTemplate("00/00/0000", "Fecha", "26102005", typeof(DateTime), culture));
                // Short Date and Time.
                maskDescriptors.Add(new MaskDescriptorTemplate("00/00/0000 00:00", "Fecha y hora", "261020051430", typeof(DateTime), culture));
                // Social Security Number.
                maskDescriptors.Add(new MaskDescriptorTemplate("000-00-0000", "Número del seguro social", "123456789", null, culture));
                // Time.
                maskDescriptors.Add(new MaskDescriptorTemplate("00:00", "Hora", "0830", typeof(DateTime), culture));
                // Zip code.
                maskDescriptors.Add(new MaskDescriptorTemplate("00000", "Código postal", "12345", null, culture));
                break;

            // case "ja-JP": // Japanese.
            case "ja": // Japanese.
                // Numeric.
                maskDescriptors.Add(new MaskDescriptorTemplate("99999", "数値（５桁）", "12345", typeof(int), culture));
                // Phone Number.
                maskDescriptors.Add(new MaskDescriptorTemplate("99900-9990-0000", "電話番号", "  012- 345-6789", null, culture));
                // Mobile Phone Number.
                maskDescriptors.Add(new MaskDescriptorTemplate("000-0000-0000", "携帯電話番号", "00001234567", null, culture));
                // Short Date.
                maskDescriptors.Add(new MaskDescriptorTemplate("0000/00/00", "日付（西暦）", "20050620", typeof(DateTime), culture));
                // Short Date and Time.
                maskDescriptors.Add(new MaskDescriptorTemplate("0000/00/00 00:00:00", "日付と時間（西暦）", "2005/06/11 04:33:22", typeof(DateTime), culture));
                // Time.
                maskDescriptors.Add(new MaskDescriptorTemplate("90:00", "時間", " 633", typeof(DateTime), culture));
                // Zip code.
                maskDescriptors.Add(new MaskDescriptorTemplate("000-0000", "郵便番号", "1820021", null, culture));
                // Calendar Gregorian.
                maskDescriptors.Add(new MaskDescriptorTemplate("0000年90月90日", "日付（西暦、日本語）", "2005年 6月11日", typeof(DateTime), culture));
                // Short date Japanese Calendar.
                maskDescriptors.Add(new MaskDescriptorTemplate("00/00/00", "日付（和暦）", "170611", typeof(DateTime), culture));
                // Calendar Japanese with Era.
                maskDescriptors.Add(new MaskDescriptorTemplate("AA90年90月90日", "日付（和暦、日本語）", "平成17年 6月11日", typeof(DateTime), culture));
                // Short Date Time Japanese.
                maskDescriptors.Add(new MaskDescriptorTemplate("0000年90月90日 90時90分", "日付と時間（日本語）", "2005年 6月11日  3時33分", typeof(DateTime), culture));
                // Short Date Time Era Calendar.
                maskDescriptors.Add(new MaskDescriptorTemplate("00/00/00 00:00:00", "日付と時間（和暦）", "170611043322", typeof(DateTime), culture));
                // Short Date Time Japanese Era Calendar.
                maskDescriptors.Add(new MaskDescriptorTemplate("AA00年90月90日 90時90分", "日付と時間（和暦、日本語）", "平成17年 6月11日  3時33分", typeof(DateTime), culture));
                // Time Japanese.
                maskDescriptors.Add(new MaskDescriptorTemplate("90時90分", "時間（日本語）", " 633", typeof(DateTime), culture));
                break;

            // case "zh-CN":  // People's Republic of China.
            case "zh-CHS":  // Simplified Chinese.
            case "zh-Hans":  // New name of Simplified Chinese.
                // Numeric.
                maskDescriptors.Add(new MaskDescriptorTemplate("99999", "数字(最长5位)", "12345", typeof(int), culture));
                // Phone Number.
                maskDescriptors.Add(new MaskDescriptorTemplate("(900)9000-0000", "（区号）电话号码", " 1234567890", null, culture));
                // Phone Number No Area Code.
                maskDescriptors.Add(new MaskDescriptorTemplate("9000-0000", "电话号码", "12345678", null, culture));
                // Mobile Phone Number.
                maskDescriptors.Add(new MaskDescriptorTemplate("000-0000-0000", "移动电话号码", "12345678901", null, culture));
                // Short Date.
                maskDescriptors.Add(new MaskDescriptorTemplate("0000-00-00", "短日期格式", "20050611", typeof(DateTime), culture));
                // Long Date.
                maskDescriptors.Add(new MaskDescriptorTemplate("0000年90月90日", "长日期格式", "20051211", typeof(DateTime), culture));
                // Short Date and Time.
                maskDescriptors.Add(new MaskDescriptorTemplate("0000-00-00 90:00:00", "短日期时间", "2005-06-11  6:30:22", typeof(DateTime), culture));
                // Long Date Time Chinese
                maskDescriptors.Add(new MaskDescriptorTemplate("0000年90月90日 90时00分", "长日期时间", "2005年 6月11日  6时33分", typeof(DateTime), culture));
                // Short Social Security Number.
                maskDescriptors.Add(new MaskDescriptorTemplate("000000-000000-000", "15位身份证号码", "123456789012345", null, culture));
                // Long Social Security Number.
                maskDescriptors.Add(new MaskDescriptorTemplate("000000-00000000-000A", "18位身份证号码", "123456789012345678", null, culture));
                // Time.
                maskDescriptors.Add(new MaskDescriptorTemplate("90:00", "时间格式", " 633", typeof(DateTime), culture));
                // Time Chinese.
                maskDescriptors.Add(new MaskDescriptorTemplate("90时90分", "中文时间格式", " 6时33分", typeof(DateTime), culture));
                // Zip code.
                maskDescriptors.Add(new MaskDescriptorTemplate("000000", "邮政编码", "100080", null, culture));
                break;

            // case "zh-TW":  // Chinese (Taiwan).
            case "zh-CHT":  // Traditional Chinese.
            case "zh-Hant":  // New name of Traditional Chinese.
                // Phone Number.
                maskDescriptors.Add(new MaskDescriptorTemplate("(00)9000-0000", "電話號碼", "01 2345678", null, culture));
                // Mobile Phone Number.
                maskDescriptors.Add(new MaskDescriptorTemplate("0000-000-000", "行動電話號碼", "1234567890", null, culture));
                // Short Date.
                maskDescriptors.Add(new MaskDescriptorTemplate("0000/00/00", "西曆簡短日期", "20050620", typeof(DateTime), culture));
                // Short Date Chinese.
                maskDescriptors.Add(new MaskDescriptorTemplate("0000年90月90日", "西曆完整日期", "2005年10月 2日", typeof(DateTime), culture));
                // Short Date and Time.
                maskDescriptors.Add(new MaskDescriptorTemplate("0000/00/00 00:00:00", "西曆簡短日期時間", "20050611043322", typeof(DateTime), culture));
                // Short Date Time Chinese.
                maskDescriptors.Add(new MaskDescriptorTemplate("0000年90月90日 90時90分", "西曆完整日期時間", "2005年 6月 2日  6時22分", typeof(DateTime), culture));
                // Social Security Number.
                maskDescriptors.Add(new MaskDescriptorTemplate("L000000000", "身分證字號", "A123456789", null, culture));
                // Time.
                maskDescriptors.Add(new MaskDescriptorTemplate("90:00", "時間格式", " 633", typeof(DateTime), culture));
                // Time Chinese.
                maskDescriptors.Add(new MaskDescriptorTemplate("90時90分", "中文時間格式", " 6時 3分", typeof(DateTime), culture));
                // Zip code.
                maskDescriptors.Add(new MaskDescriptorTemplate("99000", "3+2郵遞區號", "80407", null, culture));
                break;

            // case "ko-KR": // Korean.
            case "ko": // Korean.
                // Numeric.
                maskDescriptors.Add(new MaskDescriptorTemplate("99999", "숫자(5자리)", "12345", typeof(int), culture));
                // Phone Number.
                maskDescriptors.Add(new MaskDescriptorTemplate("(999)9000-0000", "전화 번호", "01234567890", null, culture));
                // Mobile Phone Number.
                maskDescriptors.Add(new MaskDescriptorTemplate("000-9000-0000", "휴대폰 번호", "01012345678", null, culture));
                // Phone Number No Area Code.
                maskDescriptors.Add(new MaskDescriptorTemplate("9000-0000", "지역 번호를 제외한 전화 번호", "12345678", null, culture));
                // Short Date.
                maskDescriptors.Add(new MaskDescriptorTemplate("0000-00-00", "간단한 날짜", "20050620", typeof(DateTime), culture));
                // Short date and time.
                maskDescriptors.Add(new MaskDescriptorTemplate("0000-00-00 90:00", "간단한 날짜 및 시간", "2005-06-20  9:20", typeof(DateTime), culture));
                // Long Date Time.
                maskDescriptors.Add(new MaskDescriptorTemplate("0000년90월90일 90시90분", "자세한 날짜 및 시간", "2005년 6월20일  6시33분", typeof(DateTime), culture));
                // Social Security Number.
                maskDescriptors.Add(new MaskDescriptorTemplate("000000-0000000", "주민 등록 번호", "1234561234567", null, culture));
                // Time.
                maskDescriptors.Add(new MaskDescriptorTemplate("90:00", "시간", " 633", typeof(DateTime), culture));
                // Zip code.
                maskDescriptors.Add(new MaskDescriptorTemplate("000-000", "우편 번호", "182021", null, culture));
                // Long Time.
                maskDescriptors.Add(new MaskDescriptorTemplate("90시90분", "자세한 시간", " 6시33분", typeof(DateTime), culture));
                // Long Date.
                maskDescriptors.Add(new MaskDescriptorTemplate("0000년 90월 90일", "자세한 날짜", "20050620", typeof(DateTime), culture));
                break;
        }

        return maskDescriptors.List;
    }

    /// <summary>
    /// This class is a List wrapper which accepts only valid MaskDescriptor objects.
    /// </summary>
    private class ValidMaskDescriptorList
    {
        /// <summary>
        /// Adds the specified MaskDescriptorTemplate object provided it is a valid one.
        /// </summary>
        public void Add(MaskDescriptorTemplate maskDescriptorTemplate)
        {
            if (maskDescriptorTemplate.Mask is not null)
            {
                List.Add(maskDescriptorTemplate);
            }
        }

        /// <summary>
        /// Returns a reference to the internal List object.
        /// </summary>
        public List<MaskDescriptor> List { get; } = [];
    }
}
