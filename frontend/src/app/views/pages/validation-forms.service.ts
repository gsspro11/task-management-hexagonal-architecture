import { Injectable } from '@angular/core';
import { descriptors } from 'chart.js/dist/core/core.defaults';

@Injectable()
export class ValidationFormsService {
  errorMessages: Record<string, any>;

  formRules = {
    usernameMin: 5,
    passwordMin: 6,
    passwordPattern: '(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{6,}'
  };

  formErrors = {
    firstName: '',
    lastName: '',
    username: '',
    email: '',
    password: '',
    confirmPassword: '',
  };

  constructor() {
    this.errorMessages = {
      firstName: {
        required: 'First name is required.'
      },
      lastName: {
        required: 'Last name is required.'
      },
      username: {
        required: 'Username is required',
        minLength: `Username must be ${this.formRules.usernameMin} characters or more.`,
        pattern: 'Must contain letters and/or numbers, no trailing spaces.'
      },
      email: {
        required: 'Email is required',
        email: 'Invalid email address'
      },
      password: {
        required: 'Password is required',
        pattern: 'Password must contain: numbers, uppercase and lowercase letters.',
        minLength: `Password must be at least ${this.formRules.passwordMin} characters.`
      },
      confirmPassword: {
        required: 'Password confirmation is required.',
        passwordMismatch: 'Passwords must match.'
      },
      title: {
        required: 'Title is required.'
      },
      description: {
        required: 'Description is required.'
      },
      dueDate: {
        required: 'Due date is required.'
      },
      priority: {
        required: 'Priority is required.'
      },
      status: {
        required: 'Status is required.'
      },
    };
  }
}