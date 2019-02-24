import { OnInit, OnDestroy, Component } from '@angular/core';
import { Subscription } from 'rxjs';
import { httpLoaderService } from '../../services/httpLoader.service';

@Component({
    selector: 'loader',
    templateUrl: './loader.component.html',
    styleUrls: ['./loader.component.scss']
})

export class LoaderComponent implements OnDestroy {

    private subscription: Subscription;
    show: boolean = false;

    constructor(private loaderService: httpLoaderService) { 
        this.subscription = this.loaderService.loaderSubject
            .subscribe((state: boolean) => {
                this.show = state;
            });
    }

    ngOnDestroy() {
        this.subscription.unsubscribe();
    }
}