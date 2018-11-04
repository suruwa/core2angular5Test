import { Component, Inject, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";
import { FormGroup, FormControl, FormBuilder, Validators } from "@angular/forms";

@Component({
    selector: "answer-edit",
    templateUrl: './answer-edit.component.html',
    styleUrls: ['./answer-edit.component.less']
})

export class AnswerEditComponent {
    title: string;
    answer: Answer;
    form: FormGroup;

    // this will be TRUE when editing an existing answer,
    // FALSE when creating a new one.
    editMode: boolean;

    constructor(private activatedRoute: ActivatedRoute,
                private router: Router,
                private http: HttpClient,
                private fb: FormBuilder,
                @Inject('BASE_URL') private baseUrl: string) {

        // create an empty object from the quiz interface
        this.answer = <Answer>{};

        var id = +this.activatedRoute.snapshot.params["id"];

        // check if we're in edit mode or not
        this.editMode = (this.activatedRoute.snapshot.url[1].path ===
            "edit");

        this.createForm();

        if (this.editMode)
        {
            // fetch the quiz from the server
            var url = this.baseUrl + "api/answer/" + id;
            this.http.get<Answer>(url).subscribe(res => {
                this.answer = res;
                this.title = "Edit Answer";

                this.updateForm();
            }, error => {
                console.error(error);
            });
        }
        else
        {
            this.answer.QuestionId = id;
            this.editMode = false;
            this.title = "Create a new answer";
        }
    }

    onSubmit() {
        // build a temporary question object from values
        var tempAnswer = <Answer>{};
        tempAnswer.Text = this.form.value.Text;
        tempAnswer.Value = this.form.value.Value;

        tempAnswer.QuestionId = this.answer.QuestionId;

        var url = this.baseUrl + "api/answer";

        if (this.editMode) {
            tempAnswer.Id = this.answer.Id;

            this.http
                .post<Answer>(url, tempAnswer)
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
                .put<Answer>(url, tempAnswer)
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

    createForm() {
        this.form = this.fb.group({
            Text: ['', Validators.required],
            Value: ['',
                [Validators.required,
                 Validators.min(-5),
                 Validators.max(5)
                ]
            ]
        });
    }

    updateForm() {
        this.form.setValue({
            Text: this.answer.Text || '',
            Value: this.answer.Value
        });
    }

    // Metody do wspomagania walidacji, by nie używać form.get('Title') w kółko

    // retrieve a FormControl
    getFormControl(name: string) {
        return this.form.get(name);
    }

    // returns TRUE if the FormControl is valid
    isValid(name: string) {
        var e = this.getFormControl(name);
        return e && e.valid;
    }

    // returns TRUE if the FormControl has been changed
    isChanged(name: string) {
        var e = this.getFormControl(name);
        return e && (e.dirty || e.touched);
    }

    // returns TRUE if the FormControl is invalid after user changes
    hasError(name: string) {
        var e = this.getFormControl(name);
        return e && (e.dirty || e.touched) && !e.valid;
    }
}

