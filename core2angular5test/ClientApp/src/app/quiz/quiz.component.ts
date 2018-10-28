import { Component, Input } from "@angular/core";

@Component({
  selector: "quiz",
  templateUrl: './quiz.component.html',
  styleUrls: ['./quiz.component.css']
})

export class QuizComponent {
  @Input() quiz: Quiz;
  //@Input - dekorator do bindowania zewnętrznych properties
  // appends metadata to the class hosting the affected property;
  // thanks to the metadata, Angular will know that
  // the given property is available for binding and will seamlessly allow it.
  // Without the metadata, the binding will be rejected by Angular for security reasons.
}
