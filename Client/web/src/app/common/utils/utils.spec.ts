import { Utils } from "./utils";

describe('Utils class', () => {

    describe('When array of elements is given to sort function', () => {
        it('should return the sorted array', () => {

            var unsortedArray = [
                {'name': 'Shesh', 'id': 1, 'email': 'sheshadrinath@gmail.com'},
                {'name': 'Ritesh', 'id': 2, 'email': 'rits.kc@gmail.com'},
                {'name': 'Anderson', 'id': 3, 'email': 'jamesanderson@xyz.com'}
            ];

            var sortedArray = Utils.sortArray(unsortedArray, 'name');
            expect(sortedArray.length).toBe(3);
            expect(sortedArray[0].name).toBe('Anderson');
        })
    })

})