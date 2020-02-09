
export class Utils {

    static sortArray(array: any[], sortBy: string, ascending: boolean = true) {
        if (!array) return;
        
        return array.sort((a, b) => {
            if (a[sortBy] < b[sortBy])
                return ascending ? -1: 1;
            if (a[sortBy] > b[sortBy])
                return ascending ? 1: -1;
            return 0;
        });
    }

    static DateToString(date: Date): string {
        var d = new Date(date),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

        if (month.length < 2) month = '0' + month;
        if (day.length < 2) day = '0' + day;

        return [year, month, day].join('-');
    }
}