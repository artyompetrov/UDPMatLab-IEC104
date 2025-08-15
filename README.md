# Data exchange interface between Simulink Desktop Real-Time and external SCADA systems via IEC 60870-5-104

This repository provides a tool for organizing data exchange between Simulink Desktop Real-Time and external SCADA systems that support the IEC 60870-5-104 protocol.

A detailed description of the project is given in a paper published in the proceedings of the "Energetics Through the Eyes of Youth – 2018" conference: https://drive.google.com/file/d/1gMV0PsgVT254y_iv3chqLAyWouDMo85L/view?usp=sharing

The solution is created in Microsoft Visual Studio 2017. The solution contains the following projects:
1. **UDP104** – Data exchange interface between Simulink Desktop Real-Time and external SCADA systems via IEC 60870-5-104;
2. **Configurator** – "Configurator" application with a graphical interface;
3. **SimulinkIEC104** – Library of common classes used by the "UDP104" and "Configurator" projects;
4. **lib60870** – Source code of the lib60870.NET library version 2.1.0 with minor modifications (https://github.com/mz-automation/lib60870.NET).

# License
The source code is distributed under the GNU GPLv3 license (English text: https://www.gnu.org/licenses/gpl-3.0.en.html Russian translation: http://rusgpl.ru/)

---

# Интерфейс обмена данными между Simulink Desktop Real-Time и внешними SCADA-системами по протоколу МЭК 60870-5-104

В данном репозитории представлен инструмент, позволяющий организовать обмен данными между инструментом Simulink Desktop Real-Time и внешними SCADA-системами, поддерживающими протокол МЭК 60870-5-104.

Подробное описание проекта приведено в статье, опубликованной в сборнике материалов конференции "Энергетика глазами молодежи – 2018": https://drive.google.com/file/d/1gMV0PsgVT254y_iv3chqLAyWouDMo85L/view?usp=sharing

Решение создано в Microsoft Visual Studio 2017. Пояснение по проектам входящим в состав решения:
1. UDP104 - Интерфейс обмена данными между Simulink Desktop Real-Time и внешними SCADA-системами по протоколу МЭК 60870-5-104;
2. Configurator - Приложение "Конфигуратор" с графическим интерфейсом;
3. SimulinkIEC104 - Библиотека общих классов проектов "UDP104" и "Configurator";
4. lib60870 - Исходный код библиотеки lib60870.NET версии 2.1.0 с незначительными корректировками (https://github.com/mz-automation/lib60870.NET).

# Лицензия
Исходный код распространяется по лицензии GNU (GPLv3) (Текст лицензии на английском языке: https://www.gnu.org/licenses/gpl-3.0.en.html Перевод на русский язык: http://rusgpl.ru/)
