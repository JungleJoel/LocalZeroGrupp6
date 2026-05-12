export interface CommunityMemberDTO {
  userId: string;
  firstName: string;
  lastName: string;
  avatarImageUrl: string | null;
  isManager: boolean;
  joinedAt: string;
}
