export function formatDate(value: Date): string {
    var month = value.getMonth() + 1;
    var formattedMonth = month < 10? `0${month}`: month;
    return `${ value.getFullYear() }-${ formattedMonth }-${ value.getUTCDate() < 10 ? `0${value.getUTCDate()}` : value.getUTCDate() }`;
}

export function getToday(): string {
    var value = new Date();
    var month = value.getMonth() + 1;
    var formattedMonth = month < 10? `0${month}`: month;
    return `${ value.getFullYear() }-${ formattedMonth }-${ value.getUTCDate() < 10 ? `0${value.getUTCDate()}` : value.getUTCDate() }`;
}

export function getTomorrow(): string {
    var value = new Date();
    value.setDate(value.getUTCDate() + 1);
    var month = value.getMonth() + 1;
    var formattedMonth = month < 10? `0${month}`: month;
    return `${ value.getFullYear() }-${ formattedMonth }-${ value.getUTCDate() < 10 ? `0${value.getUTCDate()}` : value.getUTCDate() }`;
}

export function convertToDateTime(value: string): Date {
    return new Date(value);
}