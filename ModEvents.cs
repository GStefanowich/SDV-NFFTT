/*
 * This software is licensed under the MIT License
 * https://github.com/GStefanowich/SDV-NFFTT
 *
 * Copyright (c) 2019 Gregory Stefanowich
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;

namespace NotFarFromTheTree {
    public static class ModEvents {
        
        /*
         * Event Handlers
         */
        public static void OnMessageNotification(object sender, ModMessageReceivedEventArgs e) {
            if (e.FromModID != ModEntry.MOD_ID) return;
            
            switch (e.Type) {
                case "ChildUpdate": {
                    ChildDat childDat = e.ReadAs<ChildDat>();
                    childDat?.Save(Context.IsMainPlayer);
                    break;
                }
            }
        }
        
        /*
         * When the Command is Dispatched
         */
        public static void CommandUpdateChild(string command, string[] args) {
            // Get input length
            switch (args.Length) {
                case 0:
                    ModEntry.MONITOR.Log("Must specify a <gender> and an <NPC>.", LogLevel.Error);
                    return;
                case 1:
                case 2: {
                    // Get the inputted child
                    Child child = ChildDat.GetByName(args[0]);
                    if (child == null) {
                        ModEntry.MONITOR.Log($"Could not location a <Child> named \"{args[0]}\".", LogLevel.Error);
                        return;
                    } else if (child.idOfParent.Value != Game1.player.UniqueMultiplayerID && (!Context.IsMainPlayer)) {
                        ModEntry.MONITOR.Log($"That <Child> \"{args[0]}\" does not belong to you.", LogLevel.Error);
                        return;
                    }
                    
                    if (args.Length == 1) {
                        ChildDat data = ChildDat.Of(child);
                        ModEntry.MONITOR.Log($"{child.Name}'s parent is {data.ParentName} / {data.GetCurrentSprite()}", LogLevel.Info);
                    } else {
                        // Get the inputted spouse
                        NPC parent = Game1.getCharacterFromName(args[1], true);
                        if (parent == null) {
                            ModEntry.MONITOR.Log($"Specified <Parent> \"{args[1]}\" could not be located.", LogLevel.Error);
                            return;
                        }
                        
                        ChildDat data = ChildDat.Of(child, parent);
                        if (!data.Save(true))
                            Game1.addHUDMessage(new HUDMessage("Host is not using mod \"NotFarFromTheTree\". Updated Children will not save.", HUDMessage.error_type));
                        ModEntry.MONITOR.Log($"{data.ChildName} now takes after {data.ParentName}", LogLevel.Info);
                    }
                    return;
                }
                default: {
                    ModEntry.MONITOR.Log("Too many parameters.", LogLevel.Error);
                    return;
                }
            }
        }
        
    }
}