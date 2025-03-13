


// public class UpdateUserRequest
// {
//     public string ProviderId { get; set; }
//     public string FirstName { get; set; }
//     public string LastName { get; set; }
//     public string Email { get; set; }
//     public string Username { get; set; }
//     public string? PhoneNumber { get; set; }
// }

export interface AppUser {
  email: string;
}

export interface CreateUserRequest {
  name: string;
}

export interface UpdateUserRequest {
  providerId: string;
  firstName: string;
  lastName: string;
  email: string;
  username: string;
  phoneNumber?: string | null;
}

export type CreateUserError = string | undefined;

export interface UserFormData {
  firstName: string;
  lastName: string;
  email: string;
  username: string;
  phoneNumber?: string | null;
}
