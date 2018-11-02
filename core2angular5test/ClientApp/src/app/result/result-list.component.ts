import { Component, Inject, Input, OnChanges, SimpleChanges } from "@angular/core";
import { Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";

@Component({
    selector: "result-list",
    templateUrl: './result-list.component.html',
    styleUrls: ['./result-list.component.less']
})

export class ResultListComponent implements OnChanges {
    @Input() quiz: Quiz;
    results: Result[];
    title: string;

    constructor(
        private http: HttpClient,
        @Inject('BASE_URL') private baseUrl: string,
        private router: Router) {

        this.results = [];
    }

    /*
    * https:/​/​angular.​io/​api/​core/​OnChanges
    * https:/​/​angular.​io/​guide/​lifecycle-​hooks#onchanges
    */
    ngOnChanges(changes: SimpleChanges) {
        //TODO: lepszy sposób na sprawdzenie
        if (typeof changes['quiz'] !== "undefined")
        {
            // retrieve the quiz variable change info
            var change = changes['quiz'];

            // only perform the task if the value has been changed
            if (!change.isFirstChange())
            {
                // execute the http request and retrieve the result
                this.loadData();
            }
        }
    }

    loadData() {
        var url = this.baseUrl + "api/result/All/" + this.quiz.Id;
        this.http.get<Result[]>(url).subscribe(res => {
            this.results = res;
        }, error => console.error(error));
    }

    onCreate() {
        this.router.navigate(["/result/create", this.quiz.Id]);
    }

    onEdit(result: Result) {
        this.router.navigate(["/result/edit", result.Id]);
    }

    onDelete(result: Result) {
        if (confirm("Do you really want to delete this result")) {
            var url = this.baseUrl + "api/result" + result.Id;
            this.http
                .delete(url)
                .subscribe(res => {
                    console.log("Result " + result.Id + " has been deleted.");

                    //refresh the question list
                    this.loadData();
                }, error => console.log(error));
        }
    }
}
