// Area interfaces
export interface Area {
	id: string;
	profileId: string;
	governorate: Governorates;
	region: string;
	profile?: Profile;
	jobPostings?: JobPosting[];
}

export enum Governorates {
	Cairo = "Cairo",
	Alexandria = "Alexandria",
	Giza = "Giza",
	PortSaid = "PortSaid",
	Suez = "Suez",
	Luxor = "Luxor",
	Aswan = "Aswan",
	Beheira = "Beheira",
	BeniSuef = "BeniSuef",
	Dakahlia = "Dakahlia",
	Damietta = "Damietta",
	Faiyum = "Faiyum",
	Gharbia = "Gharbia",
	Ismailia = "Ismailia",
	KafrElSheikh = "KafrElSheikh",
	Matruh = "Matruh",
	Minya = "Minya",
	Monufia = "Monufia",
	NewValley = "NewValley",
	NorthSinai = "NorthSinai",
	Qalyubia = "Qalyubia",
	Qena = "Qena",
	RedSea = "RedSea",
	Sharqia = "Sharqia",
	Sohag = "Sohag",
	SouthSinai = "SouthSinai",
}

// Badge interfaces
export interface Badge {
	id: string;
	name: string;
	description: string;
	profileBadges?: ProfileBadge[];
}

// Chat interfaces
export interface Chat {
	id: string;
	createdAt: Date;
	endedAt?: Date;
	participants?: ChatParticipant[];
	medias?: ChatMedia[];
}

export interface ChatParticipant {
	chatId: string;
	profileId: string;
	chat?: Chat;
	profile?: Profile;
}

// Comment interfaces
export interface Comment {
	id: string;
	authorId: string;
	parentId?: string;
	content: string;
	createdAt: Date;
	lastModified?: Date;
	isDeleted: boolean;
	author?: Profile;
	parent?: Comment;
	replies?: Comment[];
	votes?: CommentVote[];
	media?: CommentMedia;
}

// Job interfaces
export interface Job {
	id: string;
	actualCost: number;
	startedAt: Date;
	endedAt?: Date;
	jobStatusId: string;
	workerId: string;
	authorId: string;
	jobPostingId: string;
	status?: JobStatus;
	review?: JobReview;
	jobPosting?: JobPosting;
	author?: Profile;
	worker?: Profile;
}

export interface JobStatus {
	id: string;
	status: JobStatusEnum;
	jobs?: Job[];
}

export enum JobStatusEnum {
	Pending = "Pending",
	InProgress = "InProgress",
	Completed = "Completed",
	Cancelled = "Cancelled",
}

export interface JobReview {
	id: string;
	content: string;
	rating: number;
	createdAt: Date;
	lastModified?: Date;
	isDeleted: boolean;
	workerId: string;
	customerId: string;
	jobId: string;
	worker?: Profile;
	customer?: Profile;
	job?: Job;
}

// JobCategory interface
export interface JobCategory {
	id: string;
	name: string;
	description: string;
	jobPostings?: JobPosting[];
}

// JobPosting interface
export interface JobPosting {
	id: string;
	title: string;
	description: string;
	cost: number;
	createdAt: Date;
	updatedAt?: Date;
	authorId: string;
	areaId: string;
	categoryId: string;
	author?: Profile;
	area?: Area;
	category?: JobCategory;
	job?: Job;
	medias?: JobPostingMedia[];
}

// Media interfaces
export interface Media {
	id: string;
	name: string;
	mediaUrl: string;
	createdAt: Date;
	lastModified?: Date;
	mediaTypeId: string;
	mediaType?: MediaType;
	profile?: ProfileMedia;
	post?: PostMedia;
	profileExperience?: ProfileExperienceMedia;
	comment?: CommentMedia;
	jobPosting?: JobPostingMedia;
	chat?: ChatMedia;
}

export interface MediaType {
	id: string;
	type: MediaTypeEnum;
	medias?: Media[];
}

export enum MediaTypeEnum {
	Video = "Video",
	Image = "Image",
	Audio = "Audio",
	Document = "Document",
}

export interface ChatMedia {
	id: string;
	chatId: string;
	mediaId: string;
	chat?: Chat;
	media?: Media;
}

export interface CommentMedia {
	id: string;
	commentId: string;
	mediaId: string;
	comment?: Comment;
	media?: Media;
}

export interface JobPostingMedia {
	id: string;
	jobPostingId: string;
	mediaId: string;
	jobPosting?: JobPosting;
	media?: Media;
}

export interface PostMedia {
	id: string;
	postId: string;
	mediaId: string;
	post?: Post;
	media?: Media;
}

export interface ProfileMedia {
	id: string;
	profileId: string;
	mediaId: string;
	profile?: Profile;
	media?: Media;
}

export interface ProfileExperienceMedia {
	id: string;
	profileExperienceId: string;
	mediaId: string;
	profileExperience?: ProfileExperience;
	media?: Media;
}

// Post interface
export interface Post {
	id: string;
	title: string;
	content: string;
	authorId: string;
	createdAt: Date;
	lastModified?: Date;
	isDeleted: boolean;
	author?: Profile;
	medias?: PostMedia[];
	comments?: Comment[];
	votes?: PostVote[];
	postTags?: PostTag[];
}

// Profile interfaces
export interface Profile {
	id: string;
	userId: string;
	jobTitle: string;
	bio: string;
	profilePictureUrl?: string;
	rating: number;
	ratingCount: number;

	// Navigation properties
	user?: User;
	areas?: Area[];
	experiences?: ProfileExperience[];
	medias?: ProfileMedia[];
	posts?: Post[];
	comments?: Comment[];
	jobPostings?: JobPosting[];
	jobsAsAuthor?: Job[];
	jobsAsWorker?: Job[];
	chatParticipant?: ChatParticipant;
	profileSkills?: ProfileSkill[];
	profileTags?: ProfileTag[];
	profileBadges?: ProfileBadge[];
	postVotes?: PostVote[];
	commentVotes?: CommentVote[];
}

export interface ProfileExperience {
	id: string;
	profileId: string;
	title: string;
	description: string;
	company: string;
	location: string;
	startDate: Date;
	endDate?: Date;
	profile?: Profile;
	medias?: ProfileExperienceMedia[];
}

// Skill interface
export interface Skill {
	id: string;
	name: string;
	description: string;
	profileSkills?: ProfileSkill[];
}

// Tag interface
export interface Tag {
	id: string;
	name: string;
	description: string;
	profileTags?: ProfileTag[];
	postTags?: PostTag[];
}

// User interface
export interface User {
	id: string;
	firebaseId: string;
	firstName: string;
	lastName: string;
	email: string;
	username: string;
	phoneNumber: string;
	isBanned: boolean;
	isDeleted: boolean;
	isEmailVerified: boolean;
	createdAt: Date;
	profile?: Profile;
}

// Vote interfaces
export interface PostVote {
	id: string;
	isUpvote: boolean;
	createdAt: Date;
	profileId: string;
	postId: string;
	profile?: Profile;
	post?: Post;
}

export interface CommentVote {
	id: string;
	isUpvote: boolean;
	createdAt: Date;
	commentId: string;
	profileId: string;
	profile?: Profile;
	comment?: Comment;
}

// Many-to-Many relationship interfaces
export interface PostTag {
	postId: string;
	tagId: string;
	post?: Post;
	tag?: Tag;
}

export interface ProfileBadge {
	profileId: string;
	badgeId: string;
	profile?: Profile;
	badge?: Badge;
}

export interface ProfileSkill {
	profileId: string;
	skillId: string;
	profile?: Profile;
	skill?: Skill;
}

export interface ProfileTag {
	profileId: string;
	tagId: string;
	profile?: Profile;
	tag?: Tag;
}
