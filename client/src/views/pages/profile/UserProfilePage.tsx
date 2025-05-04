// import { useNavigate, useParams } from "react-router-dom";
// import { Button } from "@/views/components/ui/button";
// import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/views/components/ui/tabs";
// import { Badge } from "@/views/components/ui/badge";
// import { useEffect, useMemo, useState } from "react";
// import { Separator } from "@/views/components/ui/separator";
// import { useGetProfileByIdQuery } from "@/features/profiles/profilesSlice";
// import { UserPlusIcon } from "lucide-react";
// import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
// import defaultProfile from "../../../assets/Profile-pic/ProfilePic.svg";
// import { Skeleton } from "@/views/components/ui/skeleton";
// import toast from "react-hot-toast";
// import PostCard from "@/views/components/common/posts/PostCard";
// import CommentCard from "@/views/components/common/comments/CommentCard";
// import { useAppSelector } from "@/app/hooks";
// import { selectAllPosts, useGetPostsQuery } from "@/features/posts/postsSlice";
// import { useGetCommentsByUserIdQuery } from "@/features/comments/commentsSlice";
// import ProfilePostCard from "@/views/components/common/posts/ProfilePostCard";
// import ProfileCommentCard from "@/views/components/common/comments/ProfileCommentCard";
//
// const UserProfilePage = () => {
// 	const { userId } = useParams<{ userId: string }>();
// 	const navigate = useNavigate();
// 	const [_, __, ___, authUser, appUser] = useIsUserLoggedIn();
// 	const {
// 		data: profile,
// 		isLoading,
// 		error,
// 	} = useGetProfileByIdQuery(userId || "");
// 	const [activeTab, setActiveTab] = useState("latest");
//
// 	// Get all posts
// 	const { data: postsData } = useGetPostsQuery();
// 	const allPosts = useAppSelector(selectAllPosts);
// 	const {
//     data: userComments,
//     isLoading: isCommentsLoading,
//     isError: isCommentsError,
//     error: commentsError
//   } = useGetCommentsByUserIdQuery(userId || "");
//
// 	console.log("userComments", userComments);
//
// 	// Filter posts by user ID
// 	const userPosts = useMemo(() => {
// 		return allPosts.filter(post => post.author.id === userId);
// 	}, [allPosts, userId]);
//
//   // Calculate total upvotes from all user posts
//   const totalUpvotes = useMemo(() => {
//     return userPosts.reduce((sum, post) => sum + post.upvotes, 0);
//   }, [userPosts]);
//
//   const totalDownvotes = useMemo(() => {
//     return userPosts.reduce((sum, post) => sum + post.downvotes, 0);
//   }, [userPosts]);
//
//   // Check if the requested profile is the current user's profile
// 	useEffect(() => {
//     console.log("ProfileId: ", userId);
// 		if (userId && authUser && appUser && userId === appUser.id) {
// 			navigate("/profile", { replace: true });
// 		}
// 	}, [userId, authUser, navigate, appUser]);
//
// 	// Show toast notification when error occurs
//   useEffect(() => {
//     if (error) {
//       toast.error("Error loading profile. This user may not exist or you may not have permission to view their profile.");
//     }
//
//     if (isCommentsError && commentsError) {
//       toast.error("Error loading user comments.");
//       console.error("Error loading comments:", commentsError);
//     }
//   }, [error, isCommentsError, commentsError]);
//
// 	const fullName = `${profile?.firstName || ""} ${profile?.lastName || ""}`;
// 	const jobTitle = profile?.jobTitle || "Expert";
// 	const location = "Giza, Egypt"; // This would come from profile data if available
// 	const bio = "No bio available";
//
// 	// Placeholder stats - these would come from API calls
// 	const stats = {
// 		questions: userPosts.length,
// 		votes: totalUpvotes - totalDownvotes,
// 		answers: userComments?.length || 0,
// 	};
//
// 	// Helper function to find post title by post ID
//   const getPostTitleById = (postId: string) => {
//     const post = allPosts.find(p => p.id === postId);
//     return post ? post.title : "Unknown Post";
//   };
//
// 	// Don't render anything if error and no profile
// 	if (error && !profile) return null;
//
// 	return (
// 		<>
// 		<div className="w-full flex justify-center">
//         <div className="mt-5 w-3/5 max-xl:w-3/5 max-lg:w-4/5 max-sm:w-full bg-white rounded-lg shadow-md border p-3">
//           {/* Profile Header */}
//           <div className="border-gray-200">
//             {/* Cover Photo */}
//             {isLoading ? (
//               <Skeleton className="h-48 rounded-t-lg" />
//             ) : (
//               <div className="h-48 bg-gray-200 rounded-t-lg"></div>
//             )}
//
//             {/* Profile Info Section */}
//             <div className="relative px-8 pb-6">
//               {/* Avatar */}
//               <div className="absolute -top-16 left-8">
//                 {isLoading ? (
//                   <Skeleton className="rounded-full w-[110px] h-[110px] border-white border-4" />
//                 ) : (
//                   <div className="flex justify-center items-center rounded-full border-white	border-4 text-white text-4xl font-bold">
//                     {profile?.profilePictureUrl ? (
//                       <img
//                         src={profile.profilePictureUrl}
//                         alt={fullName}
//                         className="rounded-full"
//                         width={110}
//                         height={110}
//                       />
//                     ) : (
//                       <img
//                         src={defaultProfile}
//                         className="rounded-full"
//                         width={110}
//                         height={110}
//                       />
//                     )}
//                   </div>
//                 )}
//               </div>
//
//               {/* Connect/Hire Button */}
//               <div className="flex justify-end pt-4">
//                 {isLoading ? (
//                   <Skeleton className="h-9 w-28" />
//                 ) : (
//                   <Button className="bg-indigo-600 hover:bg-indigo-700 text-white gap-2">
//                     <UserPlusIcon size={16} />
//                     <span>Hire Me</span>
//                   </Button>
//                 )}
//               </div>
//
//               {/* User Info */}
//               <div className="mt-12">
//                 {isLoading ? (
//                   <>
//                     <div className="flex items-center">
//                       <Skeleton className="h-8 w-48 mr-4" />
//                       <Skeleton className="h-6 w-24" />
//                     </div>
//                     <div className="flex items-center mt-2">
//                       <Skeleton className="h-5 w-32 mr-4" />
//                       <Skeleton className="h-5 w-24" />
//                     </div>
//                     <Skeleton className="h-16 w-full mt-4" />
//                   </>
//                 ) : (
//                   <>
//                     <div className="flex items-center">
//                       <h1 className="text-2xl font-bold mr-4">
//                         {fullName}
//                       </h1>
//                       <Badge className="bg-indigo-600 hover:bg-indigo-700">
//                         Top Rated
//                       </Badge>
//                     </div>
//
//                     <div className="flex items-center text-gray-500 mt-2">
//                       <span className="mr-4">{location}</span>
//                       <span>{jobTitle}</span>
//                     </div>
//
//                     <div className="mt-4 whitespace-pre-line text-gray-700">
//                       {bio}
//                     </div>
//                   </>
//                 )}
//               </div>
//             </div>
//           </div>
//
//           <Separator className="my-3" style={{ height: "2px" }} />
//
//           {/* Stats Section */}
//           <div className="grid grid-cols-3 gap-4 my-6">
//             {isLoading || isCommentsLoading ? (
//               <>
//                 <div className="text-center">
//                   <Skeleton className="h-10 w-16 mx-auto mb-2" />
//                   <Skeleton className="h-5 w-24 mx-auto" />
//                 </div>
//                 <div className="text-center">
//                   <Skeleton className="h-10 w-16 mx-auto mb-2" />
//                   <Skeleton className="h-5 w-24 mx-auto" />
//                 </div>
//                 <div className="text-center">
//                   <Skeleton className="h-10 w-16 mx-auto mb-2" />
//                   <Skeleton className="h-5 w-24 mx-auto" />
//                 </div>
//               </>
//             ) : (
//               <>
//                 <div className="text-center">
//                   <div className="text-3xl font-semibold">
//                     {stats.questions}
//                   </div>
//                   <div className="text-sm text-gray-500">Questions Asked</div>
//                 </div>
//                 <div className="text-center">
//                   <div className={`text-3xl font-semibold ${stats.votes < 0 ? 'text-red-500' : ''}`}>
//                     {stats.votes}
//                   </div>
//                   <div className="text-sm text-gray-500">Total Votes</div>
//                 </div>
//                 <div className="text-center">
//                   <div className="text-3xl font-semibold">
//                     {stats.answers}
//                   </div>
//                   <div className="text-sm text-gray-500">Given Answers</div>
//                 </div>
//               </>
//             )}
//           </div>
//
//           <Separator className="my-3" style={{ height: "2px" }} />
//
//           {/* Tabs Section */}
//           <div className="my-6">
//             {isLoading || (activeTab === "answers" && isCommentsLoading) ? (
//               <>
//                 <Skeleton className="h-10 w-full mb-6" />
//                 <div className="space-y-4">
//                   {[1, 2, 3].map((i) => (
//                     <div key={i} className="border-b pb-4">
//                       <Skeleton className="h-6 w-3/4 mb-2" />
//                       <div className="flex my-1">
//                         <Skeleton className="h-4 w-24 mr-4" />
//                         <Skeleton className="h-4 w-32" />
//                       </div>
//                       <Skeleton className="h-16 w-full" />
//                     </div>
//                   ))}
//                 </div>
//               </>
//             ) : (
//               <Tabs
//                 defaultValue="questions"
//                 onValueChange={setActiveTab}
//                 className="w-full"
//               >
//                 <TabsList className="grid grid-cols-2 mb-6">
//                   <TabsTrigger value="questions">Latest Questions</TabsTrigger>
//                   <TabsTrigger value="answers">Answered Questions</TabsTrigger>
//                 </TabsList>
//
//                 {/* Latest Questions (Posts) Tab Content */}
//                 <TabsContent value="questions" className="space-y-4">
//                   {userPosts.length > 0 ? (
//                     userPosts.map((post) => (
//                       <ProfilePostCard
//                         key={post.id}
//                         post={post}
//                       />
//                     ))
//                   ) : (
//                     <div className="text-center py-8 text-gray-500">
//                       This user hasn't asked any questions yet.
//                     </div>
//                   )}
//                 </TabsContent>
//
//                 {/* Answered Questions (Comments) Tab Content */}
//                 <TabsContent value="answers" className="space-y-4">
//                   {userComments && userComments.length > 0 ? (
//                     userComments.map((comment) => (
//                       <ProfileCommentCard
//                         key={comment.id}
//                         comment={comment}
//                         postTitle={getPostTitleById(comment.postId)}
//                       />
//                     ))
//                   ) : (
//                     <div className="text-center py-8 text-gray-500">
//                       This user hasn't answered any questions yet.
//                     </div>
//                   )}
//                 </TabsContent>
//               </Tabs>
//             )}
//           </div>
//         </div>
//       </div>
// 		</>
// 	);
// };
//
// export default UserProfilePage;


import {useNavigate, useParams} from "react-router-dom";
import {Button} from "@/views/components/ui/button";
import {Tabs, TabsContent, TabsList, TabsTrigger} from "@/views/components/ui/tabs";
import {Badge} from "@/views/components/ui/badge";
import {useEffect, useMemo, useState} from "react";
import {Separator} from "@/views/components/ui/separator";
import {useGetProfileByIdQuery} from "@/features/profiles/profilesSlice";
import {UserPlusIcon} from "lucide-react";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import defaultProfile from "../../../assets/Profile-pic/ProfilePic.svg";
import {Skeleton} from "@/views/components/ui/skeleton";
import toast from "react-hot-toast";
import {useAppSelector} from "@/app/hooks";
import {selectAllPosts, useGetPostsQuery} from "@/features/posts/postsSlice";
import {useGetCommentsByUserIdQuery} from "@/features/comments/commentsSlice";
import ProfilePostCard from "@/views/components/common/posts/ProfilePostCard";
import ProfileCommentCard from "@/views/components/common/comments/ProfileCommentCard";

const UserProfilePage = () => {
    const {userId} = useParams<{ userId: string }>();
    const navigate = useNavigate();
    const [_, __, ___, authUser, appUser] = useIsUserLoggedIn();
    const {
        data: profile,
        isLoading,
        error,
    } = useGetProfileByIdQuery(userId || "");
    const [activeTab, setActiveTab] = useState("latest");

    // Get all posts
    const {data: postsData} = useGetPostsQuery();
    const allPosts = useAppSelector(selectAllPosts);
    const {
        data: userComments,
        isLoading: isCommentsLoading,
        isError: isCommentsError,
        error: commentsError
    } = useGetCommentsByUserIdQuery(userId || "");

    console.log("userComments", userComments);

    // Filter posts by user ID
    const userPosts = useMemo(() => {
        return allPosts.filter(post => post.author.id === userId);
    }, [allPosts, userId]);

    // Calculate total upvotes from all user posts
    const totalUpvotes = useMemo(() => {
        return userPosts.reduce((sum, post) => sum + post.upvotes, 0);
    }, [userPosts]);

    const totalDownvotes = useMemo(() => {
        return userPosts.reduce((sum, post) => sum + post.downvotes, 0);
    }, [userPosts]);

    // Check if the requested profile is the current user's profile
    useEffect(() => {
        console.log("ProfileId: ", userId);
        if (userId && authUser && appUser && userId === appUser.id) {
            navigate("/profile", {replace: true});
        }
    }, [userId, authUser, navigate, appUser]);

    // Show toast notification when error occurs
    useEffect(() => {
        if (error) {
            toast.error("Error loading profile. This user may not exist or you may not have permission to view their profile.");
        }

        if (isCommentsError && commentsError) {
            toast.error("Error loading user comments.");
            console.error("Error loading comments:", commentsError);
        }
    }, [error, isCommentsError, commentsError]);

    const fullName = `${profile?.firstName || ""} ${profile?.lastName || ""}`;
    const jobTitle = profile?.jobTitle || "Expert";
    const location = "Giza, Egypt"; // This would come from profile data if available
    const bio = "No bio available";

    // Placeholder stats - these would come from API calls
    const stats = {
        questions: userPosts.length,
        votes: totalUpvotes - totalDownvotes,
        answers: userComments?.length || 0,
    };

    // Helper function to find post title by post ID
    const getPostTitleById = (postId: string) => {
        const post = allPosts.find(p => p.id === postId);
        return post ? post.title : "Unknown Post";
    };

    // Don't render anything if error and no profile
    if (error && !profile) return null;

    return (
        <>
            <div className="w-full flex justify-center">
                <div
                    className="mt-5 w-3/5 max-xl:w-3/5 max-lg:w-4/5 max-sm:w-full bg-white dark:bg-gray-800 rounded-lg shadow-md border dark:border-gray-700 p-3">
                    {/* Profile Header */}
                    <div className="border-gray-200 dark:border-gray-700">
                        {/* Cover Photo */}
                        {isLoading ? (
                            <Skeleton className="h-48 rounded-t-lg"/>
                        ) : (
                            <div className="h-48 bg-gray-200 dark:bg-gray-700 rounded-t-lg"></div>
                        )}

                        {/* Profile Info Section */}
                        <div className="relative px-8 pb-6">
                            {/* Avatar */}
                            <div className="absolute -top-16 left-8">
                                {isLoading ? (
                                    <Skeleton
                                        className="rounded-full w-[110px] h-[110px] border-white dark:border-gray-800 border-4"/>
                                ) : (
                                    <div
                                        className="flex justify-center items-center rounded-full border-white dark:border-gray-800 border-4 text-white text-4xl font-bold">
                                        {profile?.profilePictureUrl ? (
                                            <img
                                                src={profile.profilePictureUrl}
                                                alt={fullName}
                                                className="rounded-full"
                                                width={110}
                                                height={110}
                                            />
                                        ) : (
                                            <img
                                                src={defaultProfile}
                                                className="rounded-full"
                                                width={110}
                                                height={110}
                                            />
                                        )}
                                    </div>
                                )}
                            </div>

                            {/* Connect/Hire Button */}
                            <div className="flex justify-end pt-4">
                                {isLoading ? (
                                    <Skeleton className="h-9 w-28"/>
                                ) : (
                                    <Button
                                        className="bg-indigo-600 hover:bg-indigo-700 dark:bg-indigo-500 dark:hover:bg-indigo-600 text-white gap-2">
                                        <UserPlusIcon size={16}/>
                                        <span>Hire Me</span>
                                    </Button>
                                )}
                            </div>

                            {/* User Info */}
                            <div className="mt-12">
                                {isLoading ? (
                                    <>
                                        <div className="flex items-center">
                                            <Skeleton className="h-8 w-48 mr-4"/>
                                            <Skeleton className="h-6 w-24"/>
                                        </div>
                                        <div className="flex items-center mt-2">
                                            <Skeleton className="h-5 w-32 mr-4"/>
                                            <Skeleton className="h-5 w-24"/>
                                        </div>
                                        <Skeleton className="h-16 w-full mt-4"/>
                                    </>
                                ) : (
                                    <>
                                        <div className="flex items-center">
                                            <h1 className="text-2xl font-bold mr-4 dark:text-white">
                                                {fullName}
                                            </h1>
                                            <Badge
                                                className="bg-indigo-600 hover:bg-indigo-700 dark:bg-indigo-500 dark:hover:bg-indigo-600">
                                                Top Rated
                                            </Badge>
                                        </div>

                                        <div className="flex items-center text-gray-500 dark:text-gray-400 mt-2">
                                            <span className="mr-4">{location}</span>
                                            <span>{jobTitle}</span>
                                        </div>

                                        <div className="mt-4 whitespace-pre-line text-gray-700 dark:text-gray-300">
                                            {bio}
                                        </div>
                                    </>
                                )}
                            </div>
                        </div>
                    </div>

                    <Separator className="my-3 dark:bg-gray-700" style={{height: "2px"}}/>

                    {/* Stats Section */}
                    <div className="grid grid-cols-3 gap-4 my-6">
                        {isLoading || isCommentsLoading ? (
                            <>
                                <div className="text-center">
                                    <Skeleton className="h-10 w-16 mx-auto mb-2"/>
                                    <Skeleton className="h-5 w-24 mx-auto"/>
                                </div>
                                <div className="text-center">
                                    <Skeleton className="h-10 w-16 mx-auto mb-2"/>
                                    <Skeleton className="h-5 w-24 mx-auto"/>
                                </div>
                                <div className="text-center">
                                    <Skeleton className="h-10 w-16 mx-auto mb-2"/>
                                    <Skeleton className="h-5 w-24 mx-auto"/>
                                </div>
                            </>
                        ) : (
                            <>
                                <div className="text-center">
                                    <div className="text-3xl font-semibold dark:text-white">
                                        {stats.questions}
                                    </div>
                                    <div className="text-sm text-gray-500 dark:text-gray-400">Questions Asked</div>
                                </div>
                                <div className="text-center">
                                    <div
                                        className={`text-3xl font-semibold ${stats.votes < 0 ? 'text-red-500 dark:text-red-400' : 'dark:text-white'}`}>
                                        {stats.votes}
                                    </div>
                                    <div className="text-sm text-gray-500 dark:text-gray-400">Total Votes</div>
                                </div>
                                <div className="text-center">
                                    <div className="text-3xl font-semibold dark:text-white">
                                        {stats.answers}
                                    </div>
                                    <div className="text-sm text-gray-500 dark:text-gray-400">Given Answers</div>
                                </div>
                            </>
                        )}
                    </div>

                    <Separator className="my-3 dark:bg-gray-700" style={{height: "2px"}}/>

                    {/* Tabs Section */}
                    <div className="my-6">
                        {isLoading || (activeTab === "answers" && isCommentsLoading) ? (
                            <>
                                <Skeleton className="h-10 w-full mb-6"/>
                                <div className="space-y-4">
                                    {[1, 2, 3].map((i) => (
                                        <div key={i} className="border-b dark:border-gray-700 pb-4">
                                            <Skeleton className="h-6 w-3/4 mb-2"/>
                                            <div className="flex my-1">
                                                <Skeleton className="h-4 w-24 mr-4"/>
                                                <Skeleton className="h-4 w-32"/>
                                            </div>
                                            <Skeleton className="h-16 w-full"/>
                                        </div>
                                    ))}
                                </div>
                            </>
                        ) : (
                            <Tabs
                                defaultValue="questions"
                                onValueChange={setActiveTab}
                                className="w-full"
                            >
                                <TabsList className="grid grid-cols-2 mb-6 dark:bg-gray-700">
                                    <TabsTrigger
                                        value="questions"
                                        className="data-[state=active]:bg-white
									 	dark:data-[state=active]:bg-gray-600 dark:text-gray-200">
                                        Latest Questions
                                    </TabsTrigger>
                                    <TabsTrigger
                                        value="answers"
                                        className="data-[state=active]:bg-white
									 	dark:data-[state=active]:bg-gray-600 dark:text-gray-200">
                                        Answered Questions
                                    </TabsTrigger>
                                </TabsList>

                                {/* Latest Questions (Posts) Tab Content */}
                                <TabsContent value="questions" className="space-y-4">
                                    {userPosts.length > 0 ? (
                                        userPosts.map((post) => (
                                            <ProfilePostCard
                                                key={post.id}
                                                post={post}
                                            />
                                        ))
                                    ) : (
                                        <div className="text-center py-8 text-gray-500 dark:text-gray-400">
                                            This user hasn't asked any questions yet.
                                        </div>
                                    )}
                                </TabsContent>

                                {/* Answered Questions (Comments) Tab Content */}
                                <TabsContent value="answers" className="space-y-4">
                                    {userComments && userComments.length > 0 ? (
                                        userComments.map((comment) => (
                                            <ProfileCommentCard
                                                key={comment.id}
                                                comment={comment}
                                                postTitle={getPostTitleById(comment.postId)}
                                            />
                                        ))
                                    ) : (
                                        <div className="text-center py-8 text-gray-500 dark:text-gray-400">
                                            This user hasn't answered any questions yet.
                                        </div>
                                    )}
                                </TabsContent>
                            </Tabs>
                        )}
                    </div>
                </div>
            </div>
        </>
    );
};

export default UserProfilePage;