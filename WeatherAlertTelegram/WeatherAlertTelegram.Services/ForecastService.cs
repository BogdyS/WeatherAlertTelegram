using System.Text;
using WeatherAlertTelegram.Domain;
using WeatherAlertTelegram.Domain.Weather;
using WeatherAlertTelegram.Services.Abstractions;

namespace WeatherAlertTelegram.Services;

public class ForecastService : IForecastService
{
    #region Consts

    private const float NormalPressure = 1003.92f; //1003.92 гПа или 753, если мм рт ст

    #region Weather

    private enum WeatherStates
    {
        Comfort,
        Rain,
        Snow,
        Extreme
    }

    private readonly WeatherStates[] _alertListWeatherStates =
    {
        WeatherStates.Rain,
        WeatherStates.Snow,
        WeatherStates.Extreme
    };

    private readonly string[] _rainMessages =
    {
        "Сегодня ожидается дождь, обязательно возьмите зонтик!",
        "Быть бы ненастью, да дождь помешал. Защититесь и от него с помощью зонта!",
        "Дождь стучится в окно, он будто просит войти. А вы не поддавайтесь.",
        "Стучу по твоей крыше, но ты не услышишь... Если останешься дома.",
        "Есть я и ты, а все, что кроме легко уладить с помощью зонта!",
        "Дождь не потеря, а находка. Найдите время взять с собой зонт!",
        "Мокрый дождя не боится. И вы не бойтесь, но возьмите зонт!",
        "Кажется, дождь начинается. Возьмите с собой зонт!",
        "В вашем регионе ожидаются дожди, будьте бдительны!"
    };

    private readonly string[] _snowMessages =
    {
        "А снег идёт, а снег идёт, на дворе же лёд, лёд. Будьте осторожны на дорогах!",
        "Опять этот снегодождь... Будьте аккуратны!",
        "Зимой без шубы не стыдно, а холодно. Одевайтесь теплее, ожидается снегопад!",
        "В зимний холод каждый молод. Быстрее найдите теплое помещение, ожидается снегопад!",
        "Мороз любви не студит. Согрейтесь дома в тепле и уюте, ожидается снегопад!",
        "Два друга — мороз да вьюга. Ожидается снегопад и ветер,будьте осторожны!",
        "Думали снег идет? Оказалось что бежит! Будьте аккуратны!",
        "Все растает, и снег ляжет снова. Ожидается снегопад.",
        "В вашем регионе ожидается обильный снегопад, будьте осторожны!"
    };

    private readonly string[] _extremeMessages =
    {
        "В лоне великих катастроф зреет страстное желание жить. Не рискуйте жизнью, останьтесь дома!",
        "Выход из дома не стоит того, чтобы погибнуть. Не рискуйте жизнью, останьтесь дома!",
        "В вашем регионе зафиксированы чрезвычайные погодные условия. Не рискуйте жизнью, останьтесь дома!"
    };

    #endregion

    #region Visability

    private const float MinComfortVisibility = 500;
    private const float AcceptableComfortVisibility = 1000;

    private readonly string[] _visibilityMessages =
    {
        "Куда ты ведешь нас? Не видно ни зги! — Сусанину с сердцем вскричали враги. Плохая видимость на дорогах, будьте аккуратны!",
        "Без взлёта нет полёта. Плохая видимость. Воздержитесь от авиаперелетов.",
        "Прервать полет проще, чем остановить падение. Плохая видимость. Воздержитесь от авиаперелетов.",
        "Если не можешь летать, то достойно ползай. Плохая видимость. Воздержитесь от авиаперелетов.",
        "С одной стороны, туман это хорошо – можно спрятаться, но с другой стороны – в нем может спрятаться еще кто-нибудь. Плохая видимость на дорогах, будьте аккуратны!",
        "Будущее туманно, а конец - всегда близок. Плохая видимость на дорогах, будьте аккуратны!",
        "Не создавайте видимость, сегодня она плохая. Плохая видимость на дорогах, будьте аккуратны!",
        "Туман, который смог. Плохая видимость на дорогах, будьте аккуратны!",
        "Уровень видимости в вашем регионе крайне низок, будьте осторожны на дорогах!" //чс
    };

    #endregion

    #region Wind

    private const float MaxComfortWindSpeed = 6;

    private enum WindStates
    {
        Calm, // Штиль
        Quiet, // Тихий
        Light, // Легкий
        Weak, // Слабый
        Moderate, // Умеренный
        Fresh, // Свежий
        Heavy, // Сильный
        Strong, // Крепкий
        VeryStrong, // Очень крепкий
        Storm, // Шторм
        HeavyStorm, // Сильный шторм
        ViolentStorm, // Буря
        Hurricane // Ураган
    }

    private readonly string[] _windMessages =
    {
        "Ведром ветра не смеряешь. Будьте осторожны.",
        "Ветер перемен бывает только в песнях. Будьте осторожны.",
        "Ветер на море гуляет и кораблик подгоняет, чтоб собой не рисковать, лучше дома тосковать!",
        "Умный человек слово на ветер не пустит. Пообещайте себе остаться дома.",
        "Погода покажет,откуда ветер дует. Будьте осторожны.",
        "В вашем регионе ожидается сильный ветер! Будьте осторожны.",
        "В вашем регионе ожидается штормовой ветер! Будьте осторожны.",
        "В вашем регионе ожидается буря, будьте бдительны, отложите любые поездки, берегите себя и близких!",
        "В вашем регионе ожидается ураган, срочно найдите укрытие!",
        "Незначительный ветер"
    };

    #endregion

    #endregion

    private readonly Random _random = new();

    public AlertMessage? CreateAlert(WeatherObject weather, string city)
    {
        if (IsNeedAlert(weather))
        {
            return new AlertMessage
            {
                City = city,
                Message = CreateMessage(weather)
            };
        }

        return null;
    }

    private bool IsNeedAlert(WeatherObject weather)
    {
        if (weather.Visibility < MinComfortVisibility)
        {
            return true;
        }

        if (weather.Wind.Speed > MaxComfortWindSpeed)
        {
            return true;
        }

        if ((weather.Main.Humidity > 60 && weather.Main.Temp > 30)
            || (weather.Main.Humidity < 40 && weather.Main.Temp < -30)
            || weather.Main.Temp >= 45 || weather.Main.Temp <= -45)
        {
            return true;
        }

        foreach (var weatherInfo in weather.Weather)
        {
            if (_alertListWeatherStates.Any(w => w == GetWeatherState(weatherInfo)))
            {
                return true;
            }
        }

        return false;
    }

    private string CreateMessage(WeatherObject weather)
    {
        var message = new StringBuilder();

        message.AppendLine("Прогноз погоды на сегодня");

        AddMessageTemperatureWithHumidity(message, weather);
        AddMessagePressure(message, weather);
        AddMessageHumidity(message, weather);
        AddMessageWeather(message, weather.Weather);
        AddMessageWind(message, weather.Wind);
        AddMessageVisibility(message, weather.Visibility);

        return message.ToString();
    }

    private void AddMessageTemperatureWithHumidity(StringBuilder message, WeatherObject weather)
    {
        var messageTemp = (weather.Main.Humidity, weather.Main.Temp) switch
        {
            ( > 60, > 30) => $"Жаркая погода: Температура: {weather.Main.Temp}°C. Возможен тепловой удар!",
            ( < 40, < -30) => $"Холодная погода: Температура: {weather.Main.Temp}°C. Возможно переохлаждение!",
            (_, >= 45) => $"Экстремальная жара: Температура: {weather.Main.Temp}°C. Берегите себя от солнечных лучей!",
            (_, <= -45) => $"Экстремальный: холод: {weather.Main.Temp}°C. Берегите себя от обморожения!",
            _ => $"Температура: {weather.Main.Temp}°C"
        };

        message.AppendLine(messageTemp);
    }

    private void AddMessagePressure(StringBuilder message, WeatherObject weather)
    {
        var messagePressure = weather.Main.Pressure switch
        {
            < NormalPressure - 8 =>
                $"Зафиксировано низкое атмосферное давление: {weather.Main.Pressure}гПа. Возможно ухудшение самочувствия.",
            > NormalPressure + 8 =>
                $"Зафиксировано высокое атмосферное давление: {weather.Main.Pressure}гПа. Возможно ухудшение самочувствия.",
            _ => $"Комфортное атмосферное давление: {weather.Main.Pressure}гПа"
        };

        message.AppendLine(messagePressure);
    }

    private void AddMessageHumidity(StringBuilder message, WeatherObject weather)
    {
        var messageHumidity = weather.Main.Humidity switch
        {
            < 40 =>
                $"Зафиксирована низкая влажность воздуха: {weather.Main.Humidity}%. Возможно ухудшение самочувствия.",
            > 60 =>
                $"Зафиксирована высокая влажность воздуха: {weather.Main.Humidity}%. Возможно ухудшение самочувствия.",
            _ => $"Комфортная влажность воздуха: {weather.Main.Humidity}%"
        };

        message.AppendLine(messageHumidity);
    }

    private void AddMessageWeather(StringBuilder message, WeatherInfo[] weather)
    {
        foreach (var weatherInfo in weather)
        {
            var weatherState = GetWeatherState(weatherInfo);

            if (weatherState == WeatherStates.Comfort) continue;

            var weatherMessage = weatherState switch
            {
                WeatherStates.Rain => _rainMessages[_random.Next(_rainMessages.Length)],
                WeatherStates.Snow => _snowMessages[_random.Next(_snowMessages.Length)],
                WeatherStates.Extreme => _extremeMessages[_random.Next(_extremeMessages.Length)],
            };
            message.AppendLine(weatherMessage);
        }
    }

    private void AddMessageWind(StringBuilder message, WindInfo windInfo)
    {
        var windMessage = GetWindState(windInfo.Speed) switch
        {
            WindStates.Moderate or WindStates.Fresh => _windMessages[_random.Next(0, 4)],
            WindStates.Heavy or WindStates.Strong or WindStates.VeryStrong => _windMessages[5],
            WindStates.Storm or WindStates.HeavyStorm => _windMessages[6],
            WindStates.ViolentStorm => _windMessages[7],
            WindStates.Hurricane => _windMessages[8],
            _ => _windMessages[9]
        };

        message.AppendLine(windMessage);
        message.AppendLine($"Скорость ветра: {windInfo.Speed} м/с");
        message.AppendLine($"Порыв ветра: {windInfo.Gust} м/с");
    }

    private void AddMessageVisibility(StringBuilder message, float visibility)
    {
        if (visibility <= MinComfortVisibility)
        {
            message.AppendLine(_visibilityMessages[8]);
        }
        else if (visibility is <= AcceptableComfortVisibility and > MinComfortVisibility)
        {
            message.AppendLine(_visibilityMessages[_random.Next(0, 7)]);
        }

        message.AppendLine($"Видимость составляет {visibility} метров");
    }

    private WeatherStates GetWeatherState(WeatherInfo weatherInfo)
    {
        return weatherInfo.Main switch
        {
            "Rain" => WeatherStates.Rain,
            "Snow" => WeatherStates.Snow,
            "Extreme" => WeatherStates.Extreme,
            _ => WeatherStates.Comfort
        };
    }

    private WindStates GetWindState(float windSpeed)
    {
        return windSpeed switch
        {
            < 1 => WindStates.Calm,
            < 2 => WindStates.Quiet,
            < 4 => WindStates.Light,
            < 6 => WindStates.Weak,
            < 8 => WindStates.Moderate,
            < 10 => WindStates.Fresh,
            < 13 => WindStates.Heavy,
            < 16 => WindStates.Strong,
            < 19 => WindStates.VeryStrong,
            < 22 => WindStates.Storm,
            < 26 => WindStates.HeavyStorm,
            < 29 => WindStates.ViolentStorm,
            _ => WindStates.Hurricane
        };
    }
}