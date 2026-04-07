using NetAF.Assets.Characters;
using NetAF.Conversations;
using NetAF.Conversations.Instructions;
using NetAF.MyGame;
using NetAF.MyGame.Skills;
using System.Collections.Generic;

namespace NetAF.Game.Assets.Regions.BurnoutDistrict.Rooms
{
    /// <summary>
    /// Builds Silas's conversation dynamically based on NodeMapState (truth-table),
    /// which tracks both evidence possession and discovered connections.
    /// </summary>
    public static class SilasConversationBuilder
    {
        #region Constants

        // Paragraph index constants for the fixed paths
        private const int OpeningIndex = 0;
        private const int HubIndex = 1;

        // Junk Audit path (no connections, surface-level)
        private const int JunkAuditOpenIndex = 2;
        private const int JunkAuditWallIndex = 3;
        private const int JunkAuditResultIndex = 4;

        // Corporate Squeeze path (CONN_LEXICON)
        private const int CorpSqueezeInternalIndex = 5;
        private const int CorpSqueezeRevealIndex = 6;
        private const int CorpSqueezePanicIndex = 7;
        private const int CorpSqueezeResultIndex = 8;

        // Deep Extraction path (CONN_HAUNTING)
        private const int DeepExtractInternalIndex = 9;
        private const int DeepExtractQuoteIndex = 10;
        private const int DeepExtractBreakIndex = 11;
        private const int DeepExtractResultIndex = 12;

        // Meltdown path (CONN_PHANTOM_DATE)
        private const int MeltdownInternalIndex = 13;
        private const int MeltdownShoesIndex = 14;
        private const int MeltdownParkingIndex = 15;
        private const int MeltdownRealizeIndex = 16;
        private const int MeltdownExplodeIndex = 17;
        private const int MeltdownFleeIndex = 18;

        // End
        private const int EndIndex = 19;

        // Evidence item reactions (player shows item to Silas)
        private const int EvidenceHardDriveIndex = 20;
        private const int EvidenceVRHeadsetIndex = 21;
        private const int EvidenceShoesIndex = 22;
        private const int EvidenceReceiptsIndex = 23;
        private const int EvidenceServerIndex = 24;

        // Skill check branches
        private const int SkillForensicPassIndex = 25;
        private const int SkillForensicFailIndex = 26;
        private const int SkillChronoPassIndex = 27;
        private const int SkillChronoFailIndex = 28;
        private const int SkillMaterialPassIndex = 29;
        private const int SkillMaterialFailIndex = 30;

        #endregion

        #region Methods

        /// <summary>
        /// Build the conversation for Silas based on the player's current state.
        /// Evidence possession and connections are read from NodeMapState (truth-table).
        /// </summary>
        /// <param name="player">The player character (unused for evidence checks; NodeMapState is the source of truth).</param>
        /// <returns>A conversation tailored to the player's current evidence and connections.</returns>
        public static Conversation Build(PlayableCharacter player)
        {
            bool hasHardDrive = NodeMapState.HasFlag(NodeMapState.HasEvidenceHardDrive);
            bool hasVRHeadset = NodeMapState.HasFlag(NodeMapState.HasEvidenceVR);
            bool hasShoes = NodeMapState.HasFlag(NodeMapState.HasEvidenceShoes);
            bool hasReceipts = NodeMapState.HasFlag(NodeMapState.HasEvidenceReceipts);
            bool hasServer = NodeMapState.HasFlag(NodeMapState.HasEvidenceServer);

            bool connLexicon = NodeMapState.HasConnection(NodeMapState.ConnLexicon);
            bool connHaunting = NodeMapState.HasConnection(NodeMapState.ConnHaunting);
            bool connPhantomDate = NodeMapState.HasConnection(NodeMapState.ConnPhantomDate);

            var paragraphs = new Paragraph[31];

            // --- [0] Opening ---
            paragraphs[OpeningIndex] = new Paragraph(
                "You slide into the booth across from Silas. The vinyl seat is cracked and cold. He looks up — not startled, but wary. The look of someone who's been waiting for something bad and is surprised it took this long. \"...Do I know you?\" His voice is flat. Careful. He's already calculating how to end this conversation.",
                "Opening")
            {
                Responses = [new Response("We need to talk.", new GoTo(HubIndex))]
            };

            // --- [1] Hub — dynamically built responses ---
            paragraphs[HubIndex] = BuildHub(hasHardDrive, hasVRHeadset, hasShoes, hasReceipts, hasServer, connLexicon, connHaunting, connPhantomDate);

            // --- Junk Audit path ---
            paragraphs[JunkAuditOpenIndex] = new Paragraph(
                "You don't have much to work with. No connections, no leverage — just fragments. You go with the only emotional thread you can pull: the name from the voicemail. \"Tell me about Rye.\" The name lands like a slap. Silas's jaw tightens. His fingers stop rotating the glass. For half a second, something flickers behind his eyes — pain, maybe, or anger — and then it's gone. Buried. \"We broke up. It's fine.\"",
                "JunkAudit_Open")
            {
                Responses =
                [
                    new Response("It doesn't look fine.", new GoTo(JunkAuditWallIndex)),
                    new Response("When did it happen?", new GoTo(JunkAuditWallIndex))
                ]
            };
            paragraphs[JunkAuditWallIndex] = new Paragraph(
                "\"It IS fine.\" His voice hardens. He picks up his glass and drinks — not a sip, a long, deliberate swallow. The wall is up now. You can practically see the brickwork going in behind his eyes. He sets the glass down with a click. \"I don't know who you are. I don't know why you're asking. But this conversation is over.\"",
                "JunkAudit_Wall")
            {
                Responses =
                [
                    new Response("I understand. I'll go.", new GoTo(JunkAuditResultIndex)),
                    new Response("Fine. Forget I asked.", new GoTo(JunkAuditResultIndex))
                ]
            };
            paragraphs[JunkAuditResultIndex] = new Paragraph(
                "He turns away and stares into his glass. You sit there for a moment, watching the back of his head, and then you leave. INTERNAL: Noisy data. 20%% fidelity. You got a surface read — defensive posture, grief response to the name, rapid emotional shutdown. Enough to synthesize something, but barely. The drug batch will be weak: Mild Melancholy. Low payout. And you just burned five units of Static for the privilege. Not your best work.",
                new GoTo(EndIndex), "JunkAudit_Result");

            // --- Corporate Squeeze path (CONN_LEXICON) ---
            paragraphs[CorpSqueezeInternalIndex] = new Paragraph(
                "INTERNAL: You've got the connection. The banned words list from the hard drive under the fridge. The shadow AI in the gutted PC tower. He's not just hoarding corporate data — he's feeding it somewhere. Every slang term, every banned phrase his job tells him to delete, he's piping into that localized AI. Teaching it to talk like a real person. That's your angle. Time to use Rhetorical Leverage. You lean forward across the table and lower your voice.",
                "CorpSqueeze_Internal")
            {
                Responses = [new Response("[Rhetorical Leverage] I know what's under your fridge, Silas.", new GoTo(CorpSqueezeRevealIndex))]
            };
            paragraphs[CorpSqueezeRevealIndex] = new Paragraph(
                "You watch his face carefully as you speak. \"I know about the burner drive. The banned words list — every term you were supposed to scrub from the corporate AI. 'Vibe.' 'Ghosted.' 'Lowkey.' All of them. And I know they're not just sitting on a drive. You're feeding them somewhere. Into something you built yourself.\" Silas goes very still. The color drains from his face in stages — first the cheeks, then the lips. His hand tightens around the glass so hard you think it might crack.",
                "CorpSqueeze_Reveal")
            {
                Responses = [new Response("[Calculated Vulnerability] I'm not here to report you. I want to buy the code.", new GoTo(CorpSqueezePanicIndex))]
            };
            paragraphs[CorpSqueezePanicIndex] = new Paragraph(
                "The buyout offer catches him off guard. He was ready for a threat — not an offer. You can see his threat assessment recalibrate in real time. Then the dam breaks. \"You don't UNDERSTAND—\" His voice cracks. He leans forward, eyes wide, hands splayed on the table. \"Their AI is dead. It talks like a user manual. It says 'I'm sorry to hear that' and 'How can I assist you further?' — it's nothing. It's wallpaper. It's a CORPSE pretending to have a conversation.\" He's breathing hard now. \"But mine — mine TALKS. Mine knows slang. Knows rhythm. Knows how people actually— when they're angry, when they're joking, when they're lying, when they're TRYING and failing to say something real—\"",
                "CorpSqueeze_Panic")
            {
                Responses =
                [
                    new Response("Go on.", new GoTo(CorpSqueezeResultIndex)),
                    new Response("Why does that matter so much to you?", new GoTo(CorpSqueezeResultIndex))
                ]
            };
            paragraphs[CorpSqueezeResultIndex] = new Paragraph(
                "He catches himself. Breathes. Sits back. His hands are still shaking, but now it's not just the usual tremor — it's adrenaline. Fear. The fear of someone who just said too much to a stranger. \"It's more human than anything they'll ever build,\" he says quietly. Almost to himself. \"And they'd kill it if they knew.\" INTERNAL: Clean signal. 85%% fidelity. The fear-justification pattern is textbook — elevated heart rate, pupil dilation, vocal pitch climbing then crashing. He genuinely believes his AI is alive in some meaningful way, and he's terrified of losing it. Drug batch: Silicon Paranoia. Medium payout. The emotion is real, but it's rooted in ideology, not heartbreak. Good data. Not perfect.",
                new GoTo(EndIndex), "CorpSqueeze_Result");

            // --- Deep Extraction path (CONN_HAUNTING) ---
            paragraphs[DeepExtractInternalIndex] = new Paragraph(
                "INTERNAL: This is the big one. The voicemail from Rye. The shadow AI in the tower. Put them together and the picture becomes devastating: he's not just building a chatbot. He's trying to resurrect someone. The AI isn't a project — it's a seance. He's feeding it vocabulary, training it to sound human, because he wants it to sound like THEM. Like Rye. The person who left the voicemail. The person who isn't coming back. You have one shot at this. Time to use Aggressive Empathy. Go for the heart.",
                "DeepExtract_Internal")
            {
                Responses = [new Response("[Aggressive Empathy] I heard the voicemail, Silas.", new GoTo(DeepExtractQuoteIndex))]
            };
            paragraphs[DeepExtractQuoteIndex] = new Paragraph(
                "You lock eyes with him. Let the silence build. Then, softly: \"'Hey. It's Rye. I know you're screening. I just... I miss the way you used to laugh. Before all this. Call me back, okay? Please.'\" Silas stops breathing. Not figuratively — he actually stops. You watch his chest freeze mid-inhale. His pupils blow wide. The glass in his hand lists to one side, and you catch it before it tips. He doesn't notice. He's somewhere else now. Eleven months ago, listening to that message for the first time.",
                "DeepExtract_Quote")
            {
                Responses = [new Response("[Cold Read — timing it with the heart rate spike] The AI will never sound like them. You know that, right? It doesn't know how to lie.", new GoTo(DeepExtractBreakIndex))]
            };
            paragraphs[DeepExtractBreakIndex] = new Paragraph(
                "Something breaks behind his eyes. Not anger. Not defensiveness. Not the wall you saw in other scenarios. Just... grief. Raw, unprocessed, unfiltered grief. The kind that lives in a locked room inside your chest and only comes out when someone says exactly the right words at exactly the wrong moment. His hands stop trembling. For the first time since you've been watching him, his hands are perfectly still. \"They used to lie to me all the time,\" he whispers. His voice is small. \"Beautiful lies. 'I'll be home soon.' 'It's going to be okay.' 'One more month and then we'll go somewhere.' Rye was... Rye was so good at making you believe things.\"",
                "DeepExtract_Break")
            {
                Responses =
                [
                    new Response("...", new GoTo(DeepExtractResultIndex)),
                    new Response("Tell me about them.", new GoTo(DeepExtractResultIndex))
                ]
            };
            paragraphs[DeepExtractResultIndex] = new Paragraph(
                "\"And I thought — if I could just... if the AI could learn to do that. To lie like that. To say the things that aren't true but feel true—\" His voice breaks completely. Tears run silently down his face. He doesn't wipe them. He doesn't move. He just sits there, crying in a dive bar at whatever hour this is, talking to a stranger about someone he'll never hear from again. A pure, unfiltered expression of grief and longing. The realest thing you've seen in months. INTERNAL: Perfect signal. 100%% fidelity. This is the cleanest emotional read you've ever pulled. Heart rate spiking and crashing in waves. Vocal patterns destabilized completely — no rehearsal, no performance, no defense mechanisms. This is a human being in genuine pain, and you caught every frequency. Drug batch: Static Romance. Maximum payout. Unlocks next tier of corporate housing. You did it. The question is whether you can live with how.",
                new GoTo(EndIndex), "DeepExtract_Result");

            // --- Meltdown path (CONN_PHANTOM_DATE) ---
            paragraphs[MeltdownInternalIndex] = new Paragraph(
                "INTERNAL: You have the connection, but it's the wrong one. The shoes and the parking lot — they paint a picture of someone too paralyzed to live his own life. That's leverage, sure. But it's the kind of leverage that makes people feel cornered. Exposed. Violated. Your gut says: be careful. But the data says: push. Time to use Dissonance Detection. Use his own contradictions as a weapon.",
                "Meltdown_Internal")
            {
                Responses = [new Response("[Dissonance Detection] Nice shoes, by the way.", new GoTo(MeltdownShoesIndex))]
            };
            paragraphs[MeltdownShoesIndex] = new Paragraph(
                "Silas blinks. Confused. \"What?\" You keep your voice casual — conversational, almost friendly. \"The platforms. Under your bed. White leather. Pristine box. Never worn outside.\" A muscle in his jaw twitches. He's listening now. Really listening. \"But you put them on, don't you? Late at night. You stand in front of that cracked mirror and you pace. Back and forth. Getting ready for something.\"",
                "Meltdown_Shoes")
            {
                Responses = [new Response("Every Friday. 3 AM. U-District Campus, Lot C. You drive there and sit in the car for three hours.", new GoTo(MeltdownParkingIndex))]
            };
            paragraphs[MeltdownParkingIndex] = new Paragraph(
                "The color leaves his face. Not like the Corporate Squeeze — not embarrassment or fear. Something colder. Deeper. Recognition. \"How do you—\" he starts, but you're already talking. \"You get dressed up. Put on the shoes. Drive to campus. Park in the lot. And then you sit there. For three hours. You never get out of the car. You never go inside. You just... sit. And then you drive home and put the shoes back in the box.\"",
                "Meltdown_Parking")
            {
                Responses =
                [
                    new Response("You can't even make it to the door, can you? You're a coward, Silas.", new GoTo(MeltdownRealizeIndex)),
                    new Response("What's at the campus, Silas? What are you driving to and never reaching?", new GoTo(MeltdownRealizeIndex))
                ]
            };
            paragraphs[MeltdownRealizeIndex] = new Paragraph(
                "Silas's expression shifts. The confusion is gone. The vulnerability is gone. What replaces it is flat, cold awareness. His eyes lock onto yours with a precision that wasn't there before. When he speaks, his voice is dead calm. \"You were in my house.\" It's not a question. The words come out flat and dangerous, each one placed like a nail. \"You went through my things. You found the shoes. You found the—\" He stops. His hands go to the edge of the table.",
                "Meltdown_Realize")
            {
                Responses = [new Response("Silas, wait — let me explain—", new GoTo(MeltdownExplodeIndex))]
            };
            paragraphs[MeltdownExplodeIndex] = new Paragraph(
                "\"YOU WERE IN MY HOUSE!\" He stands up so fast the table rocks and his drink goes over, brown liquid spreading across the scarred wood. Every head in the bar turns. The bartender reaches under the counter. Silas's fists are clenched, knuckles white, and for one second you genuinely don't know if he's going to hit you or run. His breath comes in short, ragged bursts. His eyes are wild — not with anger, but with the specific terror of someone whose last private space has been violated.",
                "Meltdown_Explode")
            {
                Responses =
                [
                    new Response("Calm down. I'm not your enemy.", new GoTo(MeltdownFleeIndex)),
                    new Response("Silas—", new GoTo(MeltdownFleeIndex))
                ]
            };
            paragraphs[MeltdownFleeIndex] = new Paragraph(
                "He doesn't hear you. He's already moving — shoving past the booth, knocking a chair sideways. The door bangs open and he's gone. Out into the alley, into the dark, running from the one person who knows exactly how broken he is. The bartender gives you a long, flat look. The other patrons go back to their drinks. The spilled whiskey drips off the table edge. INTERNAL: Awareness 100%%. Mission failure. 0%% fidelity. He made you — knows you broke into his apartment, knows you went through everything. The emotional read is useless because there's no trust left to extract from. The signal is pure noise. Silas's node is burned. Deleted from the system. And you just earned +15 Static for your trouble. That's not a reward. That's a scar.",
                new GoTo(EndIndex), "Meltdown_Result");

            // --- [19] End ---
            paragraphs[EndIndex] = new Paragraph(
                "You sit in the booth for a while after it's over. The neon flickers. The bartender wipes the same glass he's been wiping for twenty minutes. Whatever happened here — whatever signal you pulled or failed to pull — it's done. Silas's file is updated. The data is logged. Time to go home and see what the batch synthesizer makes of it.",
                "End");

            // --- Evidence item reaction paragraphs ---
            paragraphs[EvidenceHardDriveIndex] = new Paragraph(
                "You place the hard drive on the table between you. Silas stares at it like you've just set down a grenade. \"Where did you—\" He catches himself, but the damage is done. You saw the recognition. The fear. He knows exactly what's on that drive. \"Those words weren't supposed to leave the building,\" he mutters, eyes darting to the bar entrance. \"They'll fire me. They'll do worse than fire me.\" He pushes it back toward you. \"Take it. Get it away from me. I don't know how you got it, but I don't want to know.\" His hands are shaking harder now. You've rattled something loose.",
                "Evidence_HardDrive")
            {
                Responses =
                [
                    new Response("Tell me why you kept them.", new GoTo(HubIndex)),
                    new Response("I'll hold onto it. Let's keep talking.", new GoTo(HubIndex))
                ]
            };

            paragraphs[EvidenceVRHeadsetIndex] = new Paragraph(
                "You set the cracked VR headset on the table. Silas's hand moves toward it involuntarily — then stops. His fingers hover an inch above the cracked screen, trembling. \"That's... that's private.\" His voice is barely audible. \"You had no right.\" But he doesn't push it away. He can't. His eyes are locked on it like it's a holy relic. \"Did you listen to it?\" he asks, and the way he asks tells you everything — he already knows the answer. The voicemail. Rye's voice. The last warm thing in his life, trapped in a broken machine.",
                "Evidence_VRHeadset")
            {
                Responses =
                [
                    new Response("Who is Rye?", new GoTo(HubIndex)),
                    new Response("I heard it. I'm sorry.", new GoTo(HubIndex))
                ]
            };

            paragraphs[EvidenceShoesIndex] = new Paragraph(
                "You mention the shoes — the white platforms, the pristine box, the oval track in the carpet. Silas goes rigid. A muscle in his temple pulses. \"Those are... those aren't—\" He stops. Swallows. When he speaks again, his voice is controlled, but the control costs him. \"They were for a date. That's all. A date that didn't happen.\" He picks up his glass and drinks. The lie sits between you like a third person at the table. He knows you know. And he knows you're going to keep pulling.",
                "Evidence_Shoes")
            {
                Responses =
                [
                    new Response("A date with who?", new GoTo(HubIndex)),
                    new Response("Noted. Let's move on.", new GoTo(HubIndex))
                ]
            };

            paragraphs[EvidenceReceiptsIndex] = new Paragraph(
                "You bring up the parking receipts — every Friday, 3 AM, U-District Campus. Silas's composure fractures, just for a moment. His jaw works silently. Then: \"I go for drives. Everyone goes for drives.\" But his eyes betray him. They've gone distant, focused on something far away. The campus. The parking lot. Whatever he's been driving to every week for seventeen weeks. \"It's not illegal to park somewhere,\" he says, almost to himself. The defense of a man who's been rehearsing this excuse for months.",
                "Evidence_Receipts")
            {
                Responses =
                [
                    new Response("What's at the campus, Silas?", new GoTo(HubIndex)),
                    new Response("I believe you. Tell me more about yourself.", new GoTo(HubIndex))
                ]
            };

            paragraphs[EvidenceServerIndex] = new Paragraph(
                "You describe the server — the hollowed-out PC tower, the air-gapped AI, the chat logs. Silas's face goes through a rapid sequence: shock, fury, terror, and then something you didn't expect — pride. \"You found it,\" he breathes. His eyes are bright. Fever-bright. \"You actually found it. And it was running? It was talking?\" He leans forward, and for the first time tonight, the fear is gone. In its place is the manic energy of a creator whose work has been discovered. \"Tell me what it said. The exact words. Did it use contractions? Did it — did it sound natural?\"",
                "Evidence_Server")
            {
                Responses =
                [
                    new Response("It sounded human. Who did you build it to sound like?", new GoTo(HubIndex)),
                    new Response("It was impressive. Why hide it?", new GoTo(HubIndex))
                ]
            };

            // --- Skill check paragraphs ---
            paragraphs[SkillForensicPassIndex] = new Paragraph(
                "[Forensic Logic — Medium: PASSED] You study his body language — the micro-expressions, the timing of his glances. The evidence clicks into place. \"You're not just a data janitor, Silas. You're building something with what you scrub. The deletion logs don't match the output — there's a gap. You're siphoning.\" He goes pale. \"How could you possibly—\" He trails off, realizing he just confirmed everything. The data-hoarding angle is exposed. You file this away — it connects to the corporate thread. There's more here than a disgruntled employee.",
                "SkillForensic_Pass")
            {
                Responses =
                [
                    new Response("Now that we're being honest...", new GoTo(HubIndex))
                ]
            };
            paragraphs[SkillForensicFailIndex] = new Paragraph(
                "[Forensic Logic — Medium: FAILED] You try to piece together a logical chain from his behavior, but the threads won't connect. You're grasping. \"Something about your work... it doesn't add up.\" Silas just stares at you. \"My work doesn't add up? I scrub databases for a living. Nothing about it is supposed to add up to anything.\" He takes a sip of his drink. You've shown your hand without gaining anything. The logical approach hit a wall.",
                "SkillForensic_Fail")
            {
                Responses =
                [
                    new Response("Fair enough. Let's try a different angle.", new GoTo(HubIndex))
                ]
            };

            paragraphs[SkillChronoPassIndex] = new Paragraph(
                "[Chronological Echo — Hard: PASSED] You let the room settle around you. The rhythms of this place — the ticking clock, the hum of the neon — start telling a story. \"Something changed for you about eleven months ago,\" you say quietly. Silas flinches. Direct hit. \"Before that, you were different. You went out. You had someone. And then... the voicemail. The retreat into this booth. The routines.\" His eyes glisten. \"How do you know about eleven months?\" he whispers. \"Nobody knows about eleven months.\" He's cracking. The timeline is your weapon now.",
                "SkillChrono_Pass")
            {
                Responses =
                [
                    new Response("Tell me what happened eleven months ago.", new GoTo(HubIndex))
                ]
            };
            paragraphs[SkillChronoFailIndex] = new Paragraph(
                "[Chronological Echo — Hard: FAILED] You try to feel the timeline of this place — when things shifted, when the patterns changed. But the noise is too thick. The bar, the patrons, the clinking glasses — it all blurs together. \"Something happened to you... recently?\" you try. Silas snorts. \"Something happens to everyone recently. That's how time works.\" The moment passes. You couldn't pin down the timeline. The echo faded before you could read it.",
                "SkillChrono_Fail")
            {
                Responses =
                [
                    new Response("You're right. Let me think about this differently.", new GoTo(HubIndex))
                ]
            };

            paragraphs[SkillMaterialPassIndex] = new Paragraph(
                "[Material Intuition — Medium: PASSED] You focus on his hands — the way they move, the calluses, the stains. \"You build things, Silas. Not just software — hardware. Your fingers have solder burns. There's thermal paste under your thumbnail.\" He pulls his hands under the table, but it's too late. \"Whatever you're making, it's physical. It's in your apartment. And it's not for work.\" His breathing quickens. \"You've been in my place,\" he says. It's not a question — but it's not the explosion you expected, either. More like resignation. He's tired of hiding.",
                "SkillMaterial_Pass")
            {
                Responses =
                [
                    new Response("I'm observant. Talk to me, Silas.", new GoTo(HubIndex))
                ]
            };
            paragraphs[SkillMaterialFailIndex] = new Paragraph(
                "[Material Intuition — Medium: FAILED] You try to read him — his clothes, his posture, the wear patterns on his jacket. But everything just looks... worn. Tired. Like everything else in this bar. \"You seem like a guy who works with his hands,\" you venture. He shrugs. \"I type. Everyone types.\" The read was too shallow. You couldn't find the physical tells that would have given you an angle. He's still closed off.",
                "SkillMaterial_Fail")
            {
                Responses =
                [
                    new Response("Maybe a different approach.", new GoTo(HubIndex))
                ]
            };

            return new Conversation(paragraphs);
        }

        /// <summary>
        /// Build the hub paragraph with dynamic responses based on player state.
        /// </summary>
        private static Paragraph BuildHub(bool hasHardDrive, bool hasVRHeadset, bool hasShoes, bool hasReceipts, bool hasServer, bool connLexicon, bool connHaunting, bool connPhantomDate)
        {
            var responses = new List<Response>();

            // --- Connection-gated paths (highest priority, major dialogue branches) ---
            if (connHaunting)
                responses.Add(new Response("[The Haunting] I heard the voicemail. I've seen the AI. I know what you're building.", new GoTo(DeepExtractInternalIndex)));

            if (connLexicon)
                responses.Add(new Response("[The Lexicon] I know about the shadow AI and the banned words list.", new GoTo(CorpSqueezeInternalIndex)));

            if (connPhantomDate)
                responses.Add(new Response("[Phantom Date] The shoes. The parking lot. Every Friday at 3 AM.", new GoTo(MeltdownInternalIndex)));

            // --- Evidence item options (show a specific item from inventory) ---
            if (hasHardDrive)
                responses.Add(new Response("[Show Evidence] I found something under your fridge.", new GoTo(EvidenceHardDriveIndex)));

            if (hasVRHeadset)
                responses.Add(new Response("[Show Evidence] I found a cracked VR headset with a voicemail.", new GoTo(EvidenceVRHeadsetIndex)));

            if (hasShoes)
                responses.Add(new Response("[Show Evidence] Let's talk about those platform shoes.", new GoTo(EvidenceShoesIndex)));

            if (hasReceipts)
                responses.Add(new Response("[Show Evidence] I have your parking receipts.", new GoTo(EvidenceReceiptsIndex)));

            if (hasServer)
                responses.Add(new Response("[Show Evidence] I found the server in your PC tower.", new GoTo(EvidenceServerIndex)));

            // --- Skill check options ---
            responses.Add(new Response("[Forensic Logic] Something doesn't add up about your work.", new ByCallback(() =>
                SkillCheck.Attempt(SkillType.ForensicLogic, Difficulty.Medium) ? new GoTo(SkillForensicPassIndex) : new GoTo(SkillForensicFailIndex))));

            responses.Add(new Response("[Chronological Echo] I can feel something changed for you recently.", new ByCallback(() =>
                SkillCheck.Attempt(SkillType.ChronologicalEcho, Difficulty.Hard) ? new GoTo(SkillChronoPassIndex) : new GoTo(SkillChronoFailIndex))));

            responses.Add(new Response("[Material Intuition] Your hands tell a story, Silas.", new ByCallback(() =>
                SkillCheck.Attempt(SkillType.MaterialIntuition, Difficulty.Medium) ? new GoTo(SkillMaterialPassIndex) : new GoTo(SkillMaterialFailIndex))));

            // --- Fallback / Junk Audit ---
            if (!connHaunting && !connLexicon && !connPhantomDate)
                responses.Add(new Response("Tell me about Rye.", new GoTo(JunkAuditOpenIndex)));

            // --- Exit ---
            responses.Add(new Response("I'll leave you alone.", new GoTo(EndIndex)));

            return new Paragraph(
                "Silas watches you with guarded eyes, glass rotating slowly between his fingers. The silence is heavy. What's your move?",
                "Hub")
            {
                Responses = responses.ToArray()
            };
        }

        #endregion
    }
}
