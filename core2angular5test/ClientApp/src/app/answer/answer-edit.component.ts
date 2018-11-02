import { Component, Inject, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";

@Component({
    selector: "answer-edit",
    templateUrl: './answer-edit.component.html',
    styleUrls: ['./answer-edit.component.css']
})

export class AnswerEditComponent {
    title: string;
    answer: Answer;

    // this will be TRUE when editing an existing answer,
    // FALSE when creating a new one.
    editMode: boolean;

    constructor(private activatedRoute: ActivatedRoute,
                private router: Router,
                private http: HttpClient,
                @Inject('BASE_URL') private baseUrl: string) {

        // create an empty object from the quiz interface
        this.answer = <Answer>{};

        var id = +this.activatedRoute.snapshot.params["id"];

        // check if we're in edit mode or not
        this.editMode = (this.activatedRoute.snapshot.url[1].path ===
            "edit");

        if (this.editMode)
        {
            // fetch the quiz from the server
            var url = this.baseUrl + "api/answer/" + id;
            this.http.get<Answer>(url).subscribe(res => {
                this.answer = res;
                this.title = "Edit Answer";
            }, error => {
                console.error(error);
            });
        }
        else
        {
            this.editMode = false;
            this.title = "Create a new answer";
        }
    }

    onSubmit(answer: Answer) {
        var url = this.baseUrl + "api/answer";

        if (this.editMode) {
            this.http
                .post<Answer>(url, answer)
                .subscribe(res => {
                        var v = res;
                        console.log("Answer " + v.Id + " has been updated.");
                        this.router.navigate(["home"]);
                    },
                    error1 => console.log(error1));
        }
        else
        {
            this.http
                .put<Answer>(url, answer)
                .subscribe(res => {
                        var v = res;
                        console.log("Answer " + v.Id + " has been created.");
                        this.router.navigate(["home"]);
                    },
                    error1 => console.log(error1));
        }
    }

    onBack() {
        this.router.navigate(["home"]);
    }
}

