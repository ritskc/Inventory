export function formatDate(value: Date): string {
    return `${ value.getFullYear() }-${ value.getMonth() + 1 }-${ value.getDate() }`;
}

export function getToday(): string {
    var value = new Date();
    return `${ value.getFullYear() }-${ value.getMonth() + 1 }-${ value.getDate() }`;
}

export function getTomorrow(): string {
    var value = new Date();
    return `${ value.getFullYear() }-${ value.getMonth() + 1 }-${ value.getDate() + 1 }`;
}