import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
    selector: 'app-side-panel',
    templateUrl: './side-panel.component.html',
    styleUrls: [ './side-panel.component.css' ]
})
export class SidePanelComponent implements OnInit, OnDestroy {

    public mapId: string;

    private destroy$: Subject<void> = new Subject<void>();

    constructor(private route: ActivatedRoute) {
    }

    ngOnInit() {
        this.route.params
            .pipe(takeUntil(this.destroy$))
            .subscribe(params => this.mapId = params.id);
    }

    ngOnDestroy() {
        this.destroy$.next();
        this.destroy$.complete();
    }
}
