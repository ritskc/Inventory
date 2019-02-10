
export class Utils {

    static sortArray(array: any[], sortBy: string) {
        return array.sort((a, b) => {
            if (a[sortBy] < b[sortBy])
                return -1;
            if (a[sortBy] > b[sortBy])
                return 1;
            return 0;
        });
    }

}