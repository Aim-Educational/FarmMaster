export class CharacteristicHelper {
    public static createTextForAjax(text: string): string {
        return text;
    }

    public static createTimeSpanForAjax(
        days: number | string,
        minutes: number | string,
        seconds: number | string
    ): string {
        return days + "d " + minutes + "m " + seconds + "s";
    }
}

export default CharacteristicHelper;