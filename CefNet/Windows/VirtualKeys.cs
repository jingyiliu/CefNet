using System;
using System.Collections.Generic;
using System.Text;

namespace CefNet.WinApi
{
	public enum VirtualKeys
	{
		/// <summary>Битовая маска для извлечения кода клавиши из значения клавиши.</summary>
		KeyCode = 0xFFFF,
		/// <summary>Битовая маска для извлечения модификаторов из значения клавиши.</summary>
		Modifiers = -65536,
		/// <summary>Нет нажатых клавиш.</summary>
		None = 0x0,
		/// <summary>Левая кнопка мыши.</summary>
		LButton = 0x1,
		/// <summary>Правая кнопка мыши.</summary>
		RButton = 0x2,
		/// <summary>Клавиша отмены.</summary>
		Cancel = 0x3,
		/// <summary>Средняя кнопка мыши (трехкнопочная мышь).</summary>
		MButton = 0x4,
		/// <summary>Первая кнопка мыши (пятикнопочная мышь).</summary>
		XButton1 = 0x5,
		/// <summary>Вторая кнопка мыши (пятикнопочная мышь).</summary>
		XButton2 = 0x6,
		/// <summary>Клавиша BACKSPACE.</summary>
		Back = 0x8,
		/// <summary>Клавиша TAB.</summary>
		Tab = 0x9,
		/// <summary>Клавиша LINEFEED.</summary>
		LineFeed = 0xA,
		/// <summary>Клавиша CLEAR.</summary>
		Clear = 0xC,
		/// <summary>Клавиша RETURN.</summary>
		Return = 0xD,
		/// <summary>Клавиша ВВОД.</summary>
		Enter = 0xD,
		/// <summary>Клавиша SHIFT.</summary>
		ShiftKey = 0x10,
		/// <summary>Клавиша CTRL.</summary>
		ControlKey = 0x11,
		/// <summary>Клавиша ALT.</summary>
		Menu = 0x12,
		/// <summary>Клавиша PAUSE.</summary>
		Pause = 0x13,
		/// <summary>Клавиша CAPS LOCK.</summary>
		Capital = 0x14,
		/// <summary>Клавиша CAPS LOCK.</summary>
		CapsLock = 0x14,
		/// <summary>Клавиша режима "Кана" редактора метода ввода.</summary>
		KanaMode = 0x15,
		/// <summary>Клавиша режима IME Hanguel (поддерживается для обеспечения совместимости; используйте клавишу <see langword="HangulMode" />).</summary>
		HanguelMode = 0x15,
		/// <summary>Клавиша режима "Хангыль" редактора метода ввода.</summary>
		HangulMode = 0x15,
		/// <summary>Клавиша режима "Джунджа" редактора метода ввода.</summary>
		JunjaMode = 0x17,
		/// <summary>Клавиша окончательного режима IME.</summary>
		FinalMode = 0x18,
		/// <summary>Клавиша режима "Ханджа" редактора метода ввода.</summary>
		HanjaMode = 0x19,
		/// <summary>Клавиша режима "Кандзи" редактора метода ввода.</summary>
		KanjiMode = 0x19,
		/// <summary>Клавиша ESC.</summary>
		Escape = 0x1B,
		/// <summary>Клавиша преобразования IME.</summary>
		IMEConvert = 0x1C,
		/// <summary>Клавиша без преобразования IME.</summary>
		IMENonconvert = 0x1D,
		/// <summary>Клавиша принятия IME, заменяет клавишу <see cref="F:System.Windows.Forms.Keys.IMEAceept" />.</summary>
		IMEAccept = 0x1E,
		/// <summary>Клавиша принятия IME. Является устаревшей, вместо нее используется клавиша <see cref="F:System.Windows.Forms.Keys.IMEAccept" />.</summary>
		IMEAceept = 0x1E,
		/// <summary>Клавиша изменения режима IME.</summary>
		IMEModeChange = 0x1F,
		/// <summary>Клавиша ПРОБЕЛ.</summary>
		Space = 0x20,
		/// <summary>Клавиша PAGE UP.</summary>
		Prior = 0x21,
		/// <summary>Клавиша PAGE UP.</summary>
		PageUp = 0x21,
		/// <summary>Клавиша PAGE DOWN.</summary>
		Next = 0x22,
		/// <summary>Клавиша PAGE DOWN.</summary>
		PageDown = 0x22,
		/// <summary>Клавиша END.</summary>
		End = 0x23,
		/// <summary>Клавиша HOME.</summary>
		Home = 0x24,
		/// <summary>Клавиша СТРЕЛКА ВЛЕВО.</summary>
		Left = 0x25,
		/// <summary>Клавиша СТРЕЛКА ВВЕРХ.</summary>
		Up = 0x26,
		/// <summary>Клавиша СТРЕЛКА ВПРАВО.</summary>
		Right = 0x27,
		/// <summary>Клавиша СТРЕЛКА ВНИЗ.</summary>
		Down = 0x28,
		/// <summary>Клавиша SELECT.</summary>
		Select = 0x29,
		/// <summary>Клавиша PRINT.</summary>
		Print = 0x2A,
		/// <summary>Клавиша EXECUTE.</summary>
		Execute = 0x2B,
		/// <summary>Клавиша PRINT SCREEN.</summary>
		Snapshot = 0x2C,
		/// <summary>Клавиша PRINT SCREEN.</summary>
		PrintScreen = 0x2C,
		/// <summary>Клавиша INS.</summary>
		Insert = 0x2D,
		/// <summary>Клавиша DEL.</summary>
		Delete = 0x2E,
		/// <summary>Клавиша HELP.</summary>
		Help = 0x2F,
		/// <summary>Клавиша 0.</summary>
		D0 = 0x30,
		/// <summary>Клавиша 1.</summary>
		D1 = 0x31,
		/// <summary>Клавиша 2.</summary>
		D2 = 0x32,
		/// <summary>Клавиша 3.</summary>
		D3 = 0x33,
		/// <summary>Клавиша 4.</summary>
		D4 = 0x34,
		/// <summary>Клавиша 5.</summary>
		D5 = 0x35,
		/// <summary>Клавиша 6.</summary>
		D6 = 0x36,
		/// <summary>Клавиша 7.</summary>
		D7 = 0x37,
		/// <summary>Клавиша 8.</summary>
		D8 = 0x38,
		/// <summary>Клавиша 9.</summary>
		D9 = 0x39,
		/// <summary>Клавиша A.</summary>
		A = 0x41,
		/// <summary>Клавиша B.</summary>
		B = 0x42,
		/// <summary>Клавиша C.</summary>
		C = 0x43,
		/// <summary>Клавиша D.</summary>
		D = 0x44,
		/// <summary>Клавиша E.</summary>
		E = 0x45,
		/// <summary>Клавиша F.</summary>
		F = 0x46,
		/// <summary>Клавиша G.</summary>
		G = 0x47,
		/// <summary>Клавиша H.</summary>
		H = 0x48,
		/// <summary>Клавиша I.</summary>
		I = 0x49,
		/// <summary>Клавиша J.</summary>
		J = 0x4A,
		/// <summary>Клавиша K.</summary>
		K = 0x4B,
		/// <summary>Клавиша L.</summary>
		L = 0x4C,
		/// <summary>Клавиша M.</summary>
		M = 0x4D,
		/// <summary>Клавиша N.</summary>
		N = 0x4E,
		/// <summary>Клавиша O.</summary>
		O = 0x4F,
		/// <summary>Клавиша P.</summary>
		P = 0x50,
		/// <summary>Клавиша Q.</summary>
		Q = 0x51,
		/// <summary>Клавиша R.</summary>
		R = 0x52,
		/// <summary>Клавиша S.</summary>
		S = 0x53,
		/// <summary>Клавиша T.</summary>
		T = 0x54,
		/// <summary>Клавиша U.</summary>
		U = 0x55,
		/// <summary>Клавиша V.</summary>
		V = 0x56,
		/// <summary>Клавиша W.</summary>
		W = 0x57,
		/// <summary>Клавиша X.</summary>
		X = 0x58,
		/// <summary>Клавиша Y.</summary>
		Y = 0x59,
		/// <summary>Клавиша Z.</summary>
		Z = 0x5A,
		/// <summary>Левая клавиша с логотипом Windows (клавиатура Microsoft Natural Keyboard).</summary>
		LWin = 0x5B,
		/// <summary>Правая клавиша с логотипом Windows (клавиатура Microsoft Natural Keyboard).</summary>
		RWin = 0x5C,
		/// <summary>Клавиша контекстного меню (клавиатура Microsoft Natural).</summary>
		Apps = 0x5D,
		/// <summary>Клавиша перевода компьютера в спящий режим.</summary>
		Sleep = 0x5F,
		/// <summary>Клавиша 0 на цифровой клавиатуре.</summary>
		NumPad0 = 0x60,
		/// <summary>Клавиша 1 на цифровой клавиатуре.</summary>
		NumPad1 = 0x61,
		/// <summary>Клавиша 2 на цифровой клавиатуре.</summary>
		NumPad2 = 0x62,
		/// <summary>Клавиша 3 на цифровой клавиатуре.</summary>
		NumPad3 = 0x63,
		/// <summary>Клавиша 4 на цифровой клавиатуре.</summary>
		NumPad4 = 0x64,
		/// <summary>Клавиша 5 на цифровой клавиатуре.</summary>
		NumPad5 = 0x65,
		/// <summary>Клавиша 6 на цифровой клавиатуре.</summary>
		NumPad6 = 0x66,
		/// <summary>Клавиша 7 на цифровой клавиатуре.</summary>
		NumPad7 = 0x67,
		/// <summary>Клавиша 8 на цифровой клавиатуре.</summary>
		NumPad8 = 0x68,
		/// <summary>Клавиша 9 на цифровой клавиатуре.</summary>
		NumPad9 = 0x69,
		/// <summary>Клавиша умножения.</summary>
		Multiply = 0x6A,
		/// <summary>Клавиша сложения.</summary>
		Add = 0x6B,
		/// <summary>Клавиша разделителя.</summary>
		Separator = 0x6C,
		/// <summary>Клавиша вычитания.</summary>
		Subtract = 0x6D,
		/// <summary>Клавиша десятичного разделителя.</summary>
		Decimal = 0x6E,
		/// <summary>Клавиша деления.</summary>
		Divide = 0x6F,
		/// <summary>Клавиша F1.</summary>
		F1 = 0x70,
		/// <summary>Клавиша F2.</summary>
		F2 = 0x71,
		/// <summary>Клавиша F3.</summary>
		F3 = 0x72,
		/// <summary>Клавиша F4.</summary>
		F4 = 0x73,
		/// <summary>Клавиша F5.</summary>
		F5 = 0x74,
		/// <summary>Клавиша F6.</summary>
		F6 = 0x75,
		/// <summary>Клавиша F7.</summary>
		F7 = 0x76,
		/// <summary>Клавиша F8.</summary>
		F8 = 0x77,
		/// <summary>Клавиша F9.</summary>
		F9 = 0x78,
		/// <summary>Клавиша F10.</summary>
		F10 = 0x79,
		/// <summary>Клавиша F11.</summary>
		F11 = 0x7A,
		/// <summary>Клавиша F12.</summary>
		F12 = 0x7B,
		/// <summary>Клавиша F13.</summary>
		F13 = 0x7C,
		/// <summary>Клавиша F14.</summary>
		F14 = 0x7D,
		/// <summary>Клавиша F15.</summary>
		F15 = 0x7E,
		/// <summary>Клавиша F16.</summary>
		F16 = 0x7F,
		/// <summary>Клавиша F17.</summary>
		F17 = 0x80,
		/// <summary>Клавиша F18.</summary>
		F18 = 0x81,
		/// <summary>Клавиша F19.</summary>
		F19 = 0x82,
		/// <summary>Клавиша F20.</summary>
		F20 = 0x83,
		/// <summary>Клавиша F21.</summary>
		F21 = 0x84,
		/// <summary>Клавиша F22.</summary>
		F22 = 0x85,
		/// <summary>Клавиша F23.</summary>
		F23 = 0x86,
		/// <summary>Клавиша F24.</summary>
		F24 = 0x87,
		/// <summary>Клавиша NUM LOCK.</summary>
		NumLock = 0x90,
		/// <summary>Клавиша SCROLL LOCK.</summary>
		Scroll = 0x91,
		/// <summary>Левая клавиша SHIFT.</summary>
		LShiftKey = 0xA0,
		/// <summary>Правая клавиша SHIFT.</summary>
		RShiftKey = 0xA1,
		/// <summary>Левая клавиша CTRL.</summary>
		LControlKey = 0xA2,
		/// <summary>Правая клавиша CTRL.</summary>
		RControlKey = 0xA3,
		/// <summary>Левая клавиша ALT.</summary>
		LMenu = 0xA4,
		/// <summary>Правая клавиша ALT.</summary>
		RMenu = 0xA5,
		/// <summary>Клавиша перехода назад в браузере (Windows 2000 или более поздняя версия).</summary>
		BrowserBack = 0xA6,
		/// <summary>Клавиша перехода вперед в браузере (Windows 2000 или более поздняя версия).</summary>
		BrowserForward = 0xA7,
		/// <summary>Клавиша обновления в браузере (Windows 2000 или более поздняя версия).</summary>
		BrowserRefresh = 0xA8,
		/// <summary>Клавиша остановки в браузере (Windows 2000 или более поздняя версия).</summary>
		BrowserStop = 0xA9,
		/// <summary>Клавиша поиска в браузере (Windows 2000 или более поздняя версия).</summary>
		BrowserSearch = 0xAA,
		/// <summary>Клавиша избранного в браузере (Windows 2000 или более поздняя версия).</summary>
		BrowserFavorites = 0xAB,
		/// <summary>Клавиша домашней страницы в браузере (Windows 2000 или более поздняя версия).</summary>
		BrowserHome = 0xAC,
		/// <summary>Клавиша выключения звука (Windows 2000 или более поздняя версия).</summary>
		VolumeMute = 0xAD,
		/// <summary>Клавиша уменьшения громкости (Windows 2000 или более поздняя версия).</summary>
		VolumeDown = 0xAE,
		/// <summary>Клавиша увеличения громкости (Windows 2000 или более поздняя версия).</summary>
		VolumeUp = 0xAF,
		/// <summary>Клавиша перехода на следующую запись (Windows 2000 или более поздняя версия).</summary>
		MediaNextTrack = 0xB0,
		/// <summary>Клавиша перехода на предыдущую запись (Windows 2000 или более поздняя версия).</summary>
		MediaPreviousTrack = 0xB1,
		/// <summary>Клавиша остановки воспроизведения (Windows 2000 или более поздняя версия).</summary>
		MediaStop = 0xB2,
		/// <summary>Клавиша приостановки воспроизведения (Windows 2000 или более поздняя версия).</summary>
		MediaPlayPause = 0xB3,
		/// <summary>Клавиша запуска почты (Windows 2000 или более поздняя версия).</summary>
		LaunchMail = 0xB4,
		/// <summary>Клавиша выбора записи (Windows 2000 или более поздняя версия).</summary>
		SelectMedia = 0xB5,
		/// <summary>Клавиша запуска приложения один (Windows 2000 или более поздняя версия).</summary>
		LaunchApplication1 = 0xB6,
		/// <summary>Клавиша запуска приложения два (Windows 2000 или более поздняя версия).</summary>
		LaunchApplication2 = 0xB7,
		/// <summary>Клавиша OEM с точкой с запятой на стандартной клавиатуре США (Windows 2000 или более поздняя версия).</summary>
		OemSemicolon = 0xBA,
		/// <summary>Клавиша OEM 1.</summary>
		Oem1 = 0xBA,
		/// <summary>Клавиша OEM со знаком плюса на клавиатуре любой страны или региона (Windows 2000 или более поздняя версия).</summary>
		Oemplus = 0xBB,
		/// <summary>Клавиша OEM с запятой на клавиатуре любой страны или региона (Windows 2000 или более поздняя версия).</summary>
		Oemcomma = 0xBC,
		/// <summary>Клавиша OEM со знаком минуса на клавиатуре любой страны или региона (Windows 2000 или более поздняя версия).</summary>
		OemMinus = 0xBD,
		/// <summary>Клавиша OEM с точкой на клавиатуре любой страны или региона (Windows 2000 или более поздняя версия).</summary>
		OemPeriod = 0xBE,
		/// <summary>Клавиша OEM с вопросительным знаком на стандартной клавиатуре США (Windows 2000 или более поздняя версия).</summary>
		OemQuestion = 0xBF,
		/// <summary>Клавиша OEM 2.</summary>
		Oem2 = 0xBF,
		/// <summary>Клавиша OEM со знаком тильды на стандартной клавиатуре США (Windows 2000 или более поздняя версия).</summary>
		Oemtilde = 0xC0,
		/// <summary>Клавиша OEM 3.</summary>
		Oem3 = 0xC0,
		/// <summary>Клавиша OEM с открывающей квадратной скобкой на стандартной клавиатуре США (Windows 2000 или более поздняя версия).</summary>
		OemOpenBrackets = 0xDB,
		/// <summary>Клавиша OEM 4.</summary>
		Oem4 = 0xDB,
		/// <summary>Клавиша OEM с вертикальной чертой на стандартной клавиатуре США (Windows 2000 или более поздняя версия).</summary>
		OemPipe = 0xDC,
		/// <summary>Клавиша OEM 5.</summary>
		Oem5 = 0xDC,
		/// <summary>Клавиша OEM с закрывающей квадратной скобкой на стандартной клавиатуре США (Windows 2000 или более поздняя версия).</summary>
		OemCloseBrackets = 0xDD,
		/// <summary>Клавиша OEM 6.</summary>
		Oem6 = 0xDD,
		/// <summary>Клавиша OEM с одинарной/двойной кавычкой на стандартной клавиатуре США (Windows 2000 или более поздняя версия).</summary>
		OemQuotes = 0xDE,
		/// <summary>Клавиша OEM 7.</summary>
		Oem7 = 0xDE,
		/// <summary>Клавиша OEM 8.</summary>
		Oem8 = 0xDF,
		/// <summary>Клавиша OEM с угловой скобкой или обратной косой чертой на клавиатуре RT со 102 клавишами (Windows 2000 или более поздняя версия).</summary>
		OemBackslash = 0xE2,
		/// <summary>Клавиша OEM 102.</summary>
		Oem102 = 0xE2,
		/// <summary>Клавиша PROCESS KEY.</summary>
		ProcessKey = 0xE5,
		/// <summary>Используется для передачи символов в Юникоде в виде нажатия клавиш. Значение клавиши пакета является младшим словом 32-разрядного виртуального значения клавиши, используемого для бесклавиатурных методов ввода.</summary>
		Packet = 0xE7,
		/// <summary>Клавиша ATTN.</summary>
		Attn = 0xF6,
		/// <summary>Клавиша CRSEL.</summary>
		Crsel = 0xF7,
		/// <summary>Клавиша EXSEL.</summary>
		Exsel = 0xF8,
		/// <summary>Клавиша ERASE EOF.</summary>
		EraseEof = 0xF9,
		/// <summary>Клавиша PLAY.</summary>
		Play = 0xFA,
		/// <summary>Клавиша ZOOM.</summary>
		Zoom = 0xFB,
		/// <summary>Константа, зарезервированная для использования в будущем.</summary>
		NoName = 0xFC,
		/// <summary>Клавиша PA1.</summary>
		Pa1 = 0xFD,
		/// <summary>Клавиша CLEAR.</summary>
		OemClear = 0xFE,
		/// <summary>Клавиша SHIFT.</summary>
		Shift = 0x10000,
		/// <summary>Клавиша CTRL.</summary>
		Control = 0x20000,
		/// <summary>Клавиша ALT.</summary>
		Alt = 0x40000
	}
}