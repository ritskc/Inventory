import { Injectable } from "@angular/core";
import { Subject } from 'rxjs';

@Injectable()
export class httpLoaderService {

    loaderSubject = new Subject<boolean>();

    constructor() {
        this.loaderSubject.subscribe({
            next: (value) => {
                console.log(value);
            },
            error: (error) => {
                console.error(error);
            },
            complete: () => {
                console.info('completed');
            }
        })
    }

    show() {
        this.loaderSubject.next(true);
    }

    hide() {
        this.loaderSubject.next(false);
    }
}