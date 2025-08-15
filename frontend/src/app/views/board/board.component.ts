import { Component, OnInit, signal } from '@angular/core';
import {
  ButtonCloseDirective,
  ButtonDirective,
  CardBodyComponent,
  CardComponent,
  CardHeaderComponent,
  CardTitleDirective,
  ColComponent,
  FormControlDirective,
  FormDirective,
  FormFeedbackComponent,
  FormSelectDirective,
  GutterDirective,
  InputGroupComponent,
  ModalBodyComponent,
  ModalComponent,
  ModalFooterComponent,
  ModalHeaderComponent,
  ModalTitleDirective,
  ModalToggleDirective,
  RowComponent,
  TextColorDirective,
  ThemeDirective,
  ToastBodyComponent,
  ToastComponent,
  ToasterComponent,
  ToastHeaderComponent,
} from '@coreui/angular';
import { IconDirective } from '@coreui/icons-angular';
import { AssignmentService } from '../../core/services/assignment.service';
import { TokenService } from '../../core/services/token.service';
import { TokenModel } from '../../auth/models/token.model';
import { NgIf } from '@angular/common';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { NgxMaskDirective } from 'ngx-mask';
import { format } from 'date-fns';
import { ValidationFormsService } from '../pages/validation-forms.service';
import { CommentService } from '../../core/services/comment.service';
import { UserService } from '../../core/services/user.service';
import { AssignmentModel } from '../../core/models/assignment-model';
import { CommentModel } from '../../core/models/comment-model';

@Component({
  selector: 'app-board',
  templateUrl: './board.component.html',
  styleUrls: ['./board.component.scss'],
  imports: [
    RowComponent,
    ColComponent,
    CardComponent,
    CardHeaderComponent,
    CardBodyComponent,
    CardTitleDirective,
    ButtonDirective,
    ModalBodyComponent,
    ModalComponent,
    ModalFooterComponent,
    ModalHeaderComponent,
    ModalTitleDirective,
    ModalToggleDirective,
    TextColorDirective,
    ThemeDirective,
    ButtonCloseDirective,
    FormsModule,
    FormControlDirective,
    ReactiveFormsModule,
    FormDirective,
    GutterDirective,
    NgxMaskDirective,
    FormFeedbackComponent,
    InputGroupComponent,
    NgIf,
    ToasterComponent,
    ToastComponent,
    ToastHeaderComponent,
    ToastBodyComponent,
    FormSelectDirective,
    IconDirective
  ]
})
export class BoardComponent implements OnInit {
  formErrors: any;
  submitted = false;
  idAssignmentDelete: string | null | undefined = null;

  isModalTaskVisible = false;
  isModalConfirmVisible = false;
  isModalCommentsVisible = false;

  simpleFormTask!: FormGroup;
  simpleFormComment!: FormGroup;

  decodedToken: TokenModel | null;

  comments: any[] = [];
  assignments: AssignmentModel[] = [];
  modalAssignment: AssignmentModel | null = null;

  done: AssignmentModel[] = [];
  toDo: AssignmentModel[] = [];
  inProgress: AssignmentModel[] = [];

  userNameFilteredOptions: string[] = [];

  messageError = '';
  messageSuccess = '';

  constructor(
    private formBuilder: FormBuilder,
    private userService: UserService,
    private tokenService: TokenService,
    private commentService: CommentService,
    private assignmentService: AssignmentService,
    private validationFormsService: ValidationFormsService
  ) {
    this.decodedToken = this.tokenService.get;
  }

  position = 'top-end';
  visible = signal(false);
  percentage = signal(0);

  ngOnInit(): void {
    this.formErrors = this.validationFormsService.errorMessages;
    this.createForm();

    this.getAssignments();
  }

  createForm() {
    this.simpleFormTask = this.formBuilder.group({
      title: ['', [Validators.required]],
      email: ['', [Validators.required]],
      description: ['', [Validators.required]],
      dueDate: ['', [expDateValidators]],
      priority: ['', [Validators.required]],
      status: ['', [Validators.required]],
    });

    this.simpleFormComment = this.formBuilder.group({
      description: ['', [Validators.required]],
    });
  }

  //Tasks

  onCreateTask() {
    this.messageError = '';
    this.submitted = false;
    this.isModalTaskVisible = true;
    this.modalAssignment = null;

    this.simpleFormTask.reset();
  }

  onSubmitTask() {
    if (this.onValidateTask()) {
      const value = this.simpleFormTask.get('dueDate')?.value;
      const dueDateValue = value.replace(/\//gi, '');

      const day = parseInt(dueDateValue.substring(0, 2), 10);
      const month = parseInt(dueDateValue.substring(2, 4), 10);
      const year = parseInt(dueDateValue.substring(4, 8), 10);

      const dueDate = new Date(year, month - 1, day);

      let assignment: AssignmentModel = {
        id: this.modalAssignment?.id,
        title: this.simpleFormTask.get('title')?.value,
        userName: this.simpleFormTask.get('email')?.value,
        description: this.simpleFormTask.get('description')?.value,
        dueDate: dueDate,
        priority: Number(this.simpleFormTask.get('priority')?.value),
        status: Number(this.simpleFormTask.get('status')?.value),
      };

      if (this.modalAssignment) {
        this.editTask(assignment);
      } else {
        this.registerTask(assignment);
      }
    }
  }

  onValidateTask() {
    this.messageError = '';
    this.messageSuccess = '';
    this.submitted = true;

    // stop here if form is invalid
    return this.simpleFormTask.status === 'VALID';
  }

  onCloseModalTask() {
    this.messageError = '';
    this.submitted = false;
    this.isModalTaskVisible = false;
    this.userNameFilteredOptions = [];

    this.simpleFormTask.reset();
  }

  onConfirmDeleteTask() {
    if (this.idAssignmentDelete) {
      this.assignmentService.delete(this.idAssignmentDelete!).subscribe({
        next: () => {
          this.messageSuccess = 'Assignment deleted successfully.';
          this.onCloseModalConfirm();
          this.toggleToast();
          this.getAssignments();
        },
        error: (e) => {
          this.messageSuccess = '';
          this.messageError = e.message;
        },
      });
    }
  }

  onViewTask(id: string) {
    this.modalAssignment =
      this.assignments.find(
        (assignment: { id: string | null | undefined }) => assignment.id === id
      ) || null;

    if (!this.modalAssignment) return;

    const dueDate: Date = new Date(this.modalAssignment?.dueDate!);
    const formattedDate: string = format(dueDate, 'dd/MM/yyyy');

    this.simpleFormTask.patchValue({
      title: this.modalAssignment?.title,
      email: this.decodedToken?.email,
      description: this.modalAssignment?.description,
      dueDate: formattedDate,
      priority: this.modalAssignment?.priority,
      status: this.modalAssignment?.status,
    });
    this.isModalTaskVisible = true;
    this.simpleFormTask.updateValueAndValidity();
  }

  onDeleteTask(id: string) {
    this.idAssignmentDelete = id;

    this.isModalConfirmVisible = true;
  }

  //


  //Comments

  onSubmitComment() {
    if (this.onValidateComment()) {
      let comment: CommentModel = {
        id: null,
        userId: this.decodedToken?.id!,
        assignmentId: this.modalAssignment?.id,
        description: this.simpleFormComment.get('description')?.value,
      };

      // if (this.modalAssignment) {
      //   this.editComment(comment);
      // } else {
      this.registerComment(comment);
      //}
    }
  }

  onUpdateComments() {
    this.messageError = '';
    this.submitted = false;

    this.onViewComments(this.modalAssignment?.id!);

    this.simpleFormComment.reset();
  }

  onValidateComment() {
    this.messageError = '';
    this.messageSuccess = '';
    this.submitted = true;

    // stop here if form is invalid
    return this.simpleFormComment.status === 'VALID';
  }

  onCloseModalComments() {
    this.messageError = '';
    this.submitted = false;
    this.modalAssignment = null;
    this.isModalCommentsVisible = false;

    this.simpleFormComment.reset();
  }

  onViewComments(id: string) {
    this.modalAssignment =
      this.assignments.find(
        (assignment: { id: string | null | undefined }) => assignment.id === id
      ) || null;

    this.commentService.get(id).subscribe({
      next: (res) => {
        this.comments = res;
      },
      error: () => {
        this.comments = [];
      },
    });

    this.isModalCommentsVisible = true;
  }

  //

  toggleToast() {
    this.visible.update((value) => !value);
  }

  onInputChange() {
    const inputValue = this.simpleFormTask.get('email')?.value;
    if (inputValue.length >= 1) {
      this.userService.getByUserName(inputValue).subscribe((res) => {
        this.userNameFilteredOptions = res;
      });
    } else {
      this.userNameFilteredOptions = [];
    }
  }

  onCloseModalConfirm() {
    this.messageError = '';
    this.submitted = false;
    this.idAssignmentDelete = null;
    this.isModalConfirmVisible = false;
  }

  selectOption(option: string) {
    this.simpleFormTask.get('email')?.setValue(option);
    this.userNameFilteredOptions = [];
  }

  onTimerChange($event: number) {
    this.percentage.set($event * 25);
  }

  onVisibleChange($event: boolean) {
    this.visible.set($event);
    this.percentage.set(this.visible() ? this.percentage() : 0);
  }


  private getAssignments() {
    this.assignmentService.get(this.decodedToken?.id!).subscribe((res) => {
      this.assignments = res;

      this.assignments.forEach((element) => {
        switch (element.priority) {
          case 1:
            element.priorityDescription = 'High';
            break;
          case 2:
            element.priorityDescription = 'Medium';
            break;
          case 3:
            element.priorityDescription = 'Low';
            break;
        };

        element.truncatedDescription = truncateText(element.description, 100);
      });

      this.toDo = res.filter(
        (assignment: { status: number }) => assignment.status === 0
      );
      this.inProgress = res.filter(
        (assignment: { status: number }) => assignment.status === 1
      );
      this.done = res.filter(
        (assignment: { status: number }) => assignment.status === 2
      );
    });
  }


  private editTask(assignment: AssignmentModel) {
    this.assignmentService.edit(assignment.id!, assignment).subscribe({
      next: () => {
        this.messageError = '';
        this.messageSuccess = 'Assignment updated successfully.';
        this.onCloseModalTask();
        this.toggleToast();
        this.getAssignments();
      },
      error: (e) => {
        this.messageSuccess = '';
        this.messageError = e.message;
      },
    });
  }

  private registerTask(assignment: AssignmentModel) {
    this.assignmentService.register(assignment).subscribe({
      next: () => {
        this.messageError = '';
        this.messageSuccess = 'Assignment registered successfully.';
        this.onCloseModalTask();
        this.toggleToast();
        this.getAssignments();
      },
      error: (e) => {
        this.messageSuccess = '';
        this.messageError = e.message;
      },
    });
  }


  private editComment(comment: CommentModel) {
    this.commentService.edit(comment.id!, comment).subscribe({
      next: () => {
        this.messageError = '';
        this.messageSuccess = 'Comment updated successfully.';
        this.onUpdateComments();
        this.toggleToast();
      },
      error: (e) => {
        this.messageSuccess = '';
        this.messageError = e.message;
      },
    });
  }

  private registerComment(comment: CommentModel) {
    this.commentService.register(comment).subscribe({
      next: () => {
        this.messageError = '';
        this.messageSuccess = 'Comment registered successfully.';
        this.onUpdateComments();
        this.toggleToast();
      },
      error: (e) => {
        this.messageSuccess = '';
        this.messageError = e.message;
      },
    });
  }
  
}

function truncateText(text: string, length: number): string {
  if (text.length > length) {
      return text.substring(0, length) + "...";
  } else {
      return text;
  }
}

function expDateValidators(c: FormControl) {
  if (!c.value) {
    return {
      validateInput: {
        valid: false,
      },
    };
  }

  const value = c.value.replace(/\//gi, '');

  if (value.length === 8) {
    const day = parseInt(value.substring(0, 2), 10);
    const month = parseInt(value.substring(2, 4), 10);
    const year = parseInt(value.substring(4, 8), 10);

    // Create a date object
    const date = new Date(year, month - 1, day);

    // Validate the date
    if (
      date.getFullYear() === year &&
      date.getMonth() === month - 1 &&
      date.getDate() === day
    ) {
      return null; // Valid date
    }
  }
  return {
    validateInput: {
      valid: false,
    },
  };
}
