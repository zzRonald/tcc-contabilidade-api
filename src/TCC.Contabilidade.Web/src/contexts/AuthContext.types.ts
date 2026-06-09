import { type User, type LoginRequest, type RegisterWithInviteRequest } from '../interfaces/auth';

export interface AuthContextData {
  user: User | null;
  loading: boolean;
  authenticated: boolean;
  signIn: (data: LoginRequest) => Promise<void>;
  signOut: () => void;
  signUpWithInvite: (data: RegisterWithInviteRequest) => Promise<void>;
  updateUser: (updatedUser: User) => void;
}
