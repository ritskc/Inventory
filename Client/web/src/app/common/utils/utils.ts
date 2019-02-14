
export class Utils {

    static sortArray(array: any[], sortBy: string, ascending: boolean = true) {
        return array.sort((a, b) => {
            if (a[sortBy] < b[sortBy])
                return ascending ? -1: 1;
            if (a[sortBy] > b[sortBy])
                return ascending ? 1: -1;
            return 0;
        });
    }

}