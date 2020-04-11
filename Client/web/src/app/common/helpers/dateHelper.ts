export function formatDate(value: Date): string {
    var month = value.getMonth() + 1;
    var formattedMonth = month < 10? `0${month}`: month;
    return `${ value.getFullYear() }-${ formattedMonth }-${ value.getDate() < 10 ? `0${value.getDate()}` : value.getDate() }`;
}

export function getToday(): string {
    var value = new Date();
    var month = value.getMonth() + 1;
    var formattedMonth = month < 10? `0${month}`: month;
    return `${ value.getFullYear() }-${ formattedMonth }-${ value.getDate() < 10 ? `0${value.getDate()}` : value.getDate() }`;
}

export function getTomorrow(): string {
    var value = new Date();
    value.setDate(value.getDate() + 1);
    var month = value.getMonth() + 1;
    var formattedMonth = month < 10? `0${month}`: month;
    return `${ value.getFullYear() }-${ formattedMonth }-${ value.getDate() < 10 ? `0${value.getDate()}` : value.getDate() }`;
}

export function convertToDateTime(value: string): Date {
    return new Date(value);
}